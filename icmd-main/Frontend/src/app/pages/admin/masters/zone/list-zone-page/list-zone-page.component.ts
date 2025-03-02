import {
  download,
  generateCsv,
  mkConfig
} from 'export-to-csv';
import { ToastrService } from 'ngx-toastr';
import {
  combineLatest,
  BehaviorSubject,
  Subject
} from 'rxjs';
import {
  take,
  takeUntil
} from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { ColumnSelectorDialogsService } from 'src/app/service/column-selector';
import {
  CommonService,
  DialogsService
} from 'src/app/service/common';
import {
  ZoneDialogsService,
  ZoneSearchHelperService,
  ZoneService
} from 'src/app/service/zone';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import {
  ListZoneTableComponent,
  ZoneInfoDtoModel
} from '@c/masters/zone/list-zone-table';
import { ZoneBulkDialogComponent } from '@c/shared/bulkDelete-dialog/project-master/zone-master/zone-bulk-dialog.component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import { CustomFieldSearchModel } from '@m/common';
import {
  importZoneFileColumns,
  masterZoneListTableColumn
} from '@u/constants';
import { listColumnMemoryCacheKey } from '@u/default';
import { ExcelHelper } from '@u/helper';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-zone-page",
    templateUrl: "./list-zone-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListZoneTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        ZoneService,
        ZoneSearchHelperService,
        DialogsService,
        ZoneDialogsService,
        ExcelHelper,
        CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListZonePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListZoneTableComponent) zoneTable: ListZoneTableComponent;
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected zoneListColumns = [...masterZoneListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _zoneSearchHelperService: ZoneSearchHelperService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        private _zoneService: ZoneService,
        private _zoneDialogService: ZoneDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper, private _commonService: CommonService, private _cd: ChangeDetectorRef,
        private _columnSelectorDialogService: ColumnSelectorDialogsService) {
        this.getZoneData();
    }

    ngAfterViewInit(): void {
        this.zoneTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._zoneSearchHelperService.updateSortingChange(res);
        });

        this.zoneTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._zoneSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._zoneSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getZoneData();
                this.getMemoryCacheItem();
            }
        });

        this.zoneTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._zoneSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this zone?",
            "Confirm"
        );
        if (isOk) {
            this._zoneService.deleteZone($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getZoneData();
                    } else {
                        this._toastr.error(res.message);
                    }
                },
                (errorRes) => {
                    this._toastr.error(errorRes?.error?.message);
                }
            );
        }
    }

     // #region Delete Bulk
     protected async deleteBulkZone(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(ZoneBulkDialogComponent, {
            width: '600px',
            data: ids,
        });

          dialogRef.afterClosed().subscribe((result: string[] | null) => {

            if (result) {
                this._zoneService.deleteBulkZone(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                            this.getZoneData();
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes) => {
                        this._toastr.error(errorRes?.error?.message);
                    }
                );
            }
          });
    }

    protected async addEditZoneDialog(event: string = null): Promise<void> {
        await this._zoneDialogService.openZoneDialog(event, this.projectId);
        this.getZoneData();
    }

    protected async bulkEditZone(): Promise<void> {
        const fileName = "Edit_Zone"

        this.defaultCustomFilter(true, this.columnFilterList);
        this._zoneSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping: Record<string, string> = [{ key: 'id', label: 'Id' }]
                .concat(masterZoneListTableColumn.filter(({ key }) => key !== 'actions'))
                .reduce((acc, { key, label }) => {
                    acc[`${key}`] = `${label}`;
                    return acc;
                }, {} as Record<string, string>);

                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected exportData(): void {
        const fileName = 'Export_Zones';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._zoneSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.zoneListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.zoneListColumns, listColumnMemoryCacheKey.zones);
        let selectedColumn = masterZoneListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.zoneTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getZoneData();
        }
    }

    private getZoneData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();

            this._zoneSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [
            { fieldName: "projectIds", fieldValue: this.projectId, searchType: SearchType.Contains, isColumnFilter: false },
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        ];

        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)

        this.customFilters$.next(filters);
    }
    private tableColumnchanges() {
        this._cd.detectChanges();
        this.zoneTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.zoneTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.zoneTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.zones).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.zoneTable.displayedColumns = [...this.selectedColumns, masterZoneListTableColumn[masterZoneListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.zoneListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

     //#region Import Functionality
     protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Zone', columnHeaders: importZoneFileColumns, fieldSeparator: "," });
        const csv = generateCsv(csvConfig)([]);
        download(csvConfig)(csv);
    }

    protected onFileSelected(event: any): void {
        if (!event) return;

        const selectedFile = event.target.files[0] ?? null;
        if (!selectedFile) {
            this._toastr.error("Please select a file for import.");
            this.clearFileInput();
            return;
        }

        if (!this.projectId) {
            this._toastr.error("Please select a project.");
            this.clearFileInput();
            return;
        }

        this._zoneService.validateImportZone(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (!res || !res.isSucceeded) {
                        this._toastr.error("Validation failed. Please check your file.");
                        this.clearFileInput();
                        return;
                    } 

                    const dialogRef = this.dialog.open(ImportPreviewDialogComponent, {
                        width: '750px',
                        data: res.records
                    });

                    dialogRef.afterClosed().subscribe((confirmed) => {
                        if (confirmed) {
                            this.proceedImport(selectedFile);
                            this.getZoneData();
                        } else {
                            this.clearFileInput();
                        }
                    });

                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "File validation failed.")
                }
        });
    }

    private proceedImport(selectedFile: File): void {

        this._zoneService.importZone(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getZoneData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<ZoneInfoDtoModel>("Zone", res.records, importZoneFileColumns);
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();
                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "Import failed.");
                }
            });
    }

    private clearFileInput(): void {
        if (this.importFileInput)
            this.importFileInput.nativeElement.value = '';

        this._cd.detectChanges();
    }
    //#endregion

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}