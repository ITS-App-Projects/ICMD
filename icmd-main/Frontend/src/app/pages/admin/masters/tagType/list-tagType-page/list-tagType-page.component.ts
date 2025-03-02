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
  TagTypeDialogsService,
  TagTypeSearchHelperService,
  TagTypeService
} from 'src/app/service/tagType';

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
  ListTagTypeTableComponent,
  TagTypeInfoDtoModel
} from '@c/masters/tagType/list-tagType-table';
import { TagTypeBulkDialogComponent } from '@c/shared/bulkDelete-dialog/system-master/tag-types/tagType-bulk-dialog.component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import { CustomFieldSearchModel } from '@m/common';
import {
  importTagTypeColumns,
  masterTagTypeListTableColumn
} from '@u/constants';
import { listColumnMemoryCacheKey } from '@u/default';
import { ExcelHelper } from '@u/helper';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-tagType-page",
    templateUrl: "./list-tagType-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListTagTypeTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        TagTypeService,
        TagTypeSearchHelperService,
        DialogsService,
        ExcelHelper,
        TagTypeDialogsService, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListTagTypePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListTagTypeTableComponent) tagTypeTable: ListTagTypeTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected tagTypeListColumns = [...masterTagTypeListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _tagTypeSearchHelperService: TagTypeSearchHelperService,
        private _tagTypeService: TagTypeService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        protected appConfig: AppConfig,
        private _tagTypeDialogService: TagTypeDialogsService,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getTagTypeData();
    }

    ngAfterViewInit(): void {
        this.tagTypeTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._tagTypeSearchHelperService.updateSortingChange(res);
        });

        this.tagTypeTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._tagTypeSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._tagTypeSearchHelperService.updateFilterChange(filter);
        });

        this.tagTypeTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });

        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._tagTypeSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this tag type?",
            "Confirm"
        );
        if (isOk) {
            this._tagTypeService.deleteTagType($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTagTypeData();
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

     //#region Delete Bulk
     protected async deleteBulk(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(TagTypeBulkDialogComponent, {
            width: "600px",
            data: ids
        });

        dialogRef.afterClosed().subscribe((result: string[] | null) => {
            if (result) {
                this._tagTypeService.deleteBulkTagType(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            res.isWarning ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                            this.getTagTypeData();
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes) => {
                        this._toastr.error(errorRes?.error.message);
                    }
                );
            }
        });
    }

    protected async bulkEdit(): Promise<void> {
        const fileName = 'Edit_TagTypes';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._tagTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping: Record<string, string> = [{ key: 'id', label: 'Id' }]
                .concat(masterTagTypeListTableColumn.filter(({ key }) => key !== 'actions'))
                .reduce((acc, { key, label }) => {
                    acc[`${key}`] = `${label}`;
                    return acc;
                }, {} as Record<string, string>);

                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async addEditTagTypeDialog(event: string = null): Promise<void> {
        await this._tagTypeDialogService.openTagTypeDialog(event);
        this.getTagTypeData();
    }

    protected exportData(): void {
        const fileName = 'Export_Tag_Types';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._tagTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.tagTypeListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.tagTypeListColumns, listColumnMemoryCacheKey.tagTypes);
        let selectedColumn = masterTagTypeListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.tagTypeTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getTagTypeData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.tagTypeTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.tagTypeTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.tagTypeTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    private getTagTypeData(): void {
        this.defaultCustomFilter();
        this._tagTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        ];
        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)

        this.customFilters$.next(filters);
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.tagTypes).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.tagTypeTable.displayedColumns = [...this.selectedColumns, masterTagTypeListTableColumn[masterTagTypeListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.tagTypeListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_TagType', columnHeaders: importTagTypeColumns, fieldSeparator: "," });
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

        this._tagTypeService.validateImportTagType(selectedFile)
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
                            this.getTagTypeData();
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

        this._tagTypeService.importTagType(selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getTagTypeData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<TagTypeInfoDtoModel>("TagType", res.records, importTagTypeColumns);
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