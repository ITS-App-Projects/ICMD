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
import { DialogsService } from 'src/app/service/common';
import {
  NatureOfSignalDialogsService,
  NatureOfSignalSearchHelperService,
  NatureOfSignalService
} from 'src/app/service/natureOfSignal';

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
import { ListNatureOfSignalTableComponent } from '@c/masters/natureOfSignal/list-natureOfSignal-table';
import { NatureBulkDialogComponent } from '@c/shared/bulkDelete-dialog/system-master/nature-signals/nature-bulk-dialog-component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import {
  CustomFieldSearchModel,
  DropdownInfoDtoModel
} from '@m/common';
import { importNatureOfSignalTypeColumns } from '@u/constants';
import { ExcelHelper } from '@u/helper';

import { NatureOfSignalExportDtoModel } from './list-natureOfSignal-table-export.model';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-natureOfSignal-page",
    templateUrl: "./list-natureOfSignal-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListNatureOfSignalTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        NatureOfSignalService,
        NatureOfSignalSearchHelperService,
        DialogsService,
        ExcelHelper,
        NatureOfSignalDialogsService
    ]
})
export class ListNatureOfSignalPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListNatureOfSignalTableComponent) natureOfSignalTable: ListNatureOfSignalTableComponent;
    protected manufacturerData: DropdownInfoDtoModel[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _natureOfSignalSearchHelperService: NatureOfSignalSearchHelperService,
        private _natureOfSignalService: NatureOfSignalService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        protected appConfig: AppConfig,
        private _natureOfSignalDialogService: NatureOfSignalDialogsService,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getNatureOfSignalData();
    }

    ngAfterViewInit(): void {
        this.natureOfSignalTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._natureOfSignalSearchHelperService.updateSortingChange(res);
        });

        this.natureOfSignalTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._natureOfSignalSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._natureOfSignalSearchHelperService.updateFilterChange(filter);
        });

        this.natureOfSignalTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });

        this.tableColumnchanges();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._natureOfSignalSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this nature of signal?",
            "Confirm"
        );
        if (isOk) {
            this._natureOfSignalService.deleteNatureOfSignal($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getNatureOfSignalData();
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
        const dialogRef = this.dialog.open(NatureBulkDialogComponent, {
            width: "500px",
            data: ids
        });

        dialogRef.afterClosed().subscribe((result: string[] | null) => {
            if (result) {
                this._natureOfSignalService.deleteBulkNatureOfSignal(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            res.isWarning ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                            this.getNatureOfSignalData();
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
        const fileName = 'Edit_NatureOfSignal';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._natureOfSignalSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'id' : 'Id',
                    'name': 'Name',
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async addEditNatureOfSignalDialog(event: string = null): Promise<void> {
        await this._natureOfSignalDialogService.openNatureOfSignalDialog(event);
        this.getNatureOfSignalData();
    }

    protected exportData(): void {
        const fileName = 'Export_Nature_Of_Signals';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._natureOfSignalSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'natureOfSignalName': 'Name'
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    private getNatureOfSignalData(): void {
        this.defaultCustomFilter();
        this._natureOfSignalSearchHelperService
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

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.natureOfSignalTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.natureOfSignalTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.natureOfSignalTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_NatureOfSignal', columnHeaders: importNatureOfSignalTypeColumns, fieldSeparator: "," });
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

        this._natureOfSignalService.validateImportNatureOfSignal(selectedFile)
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
                            this.getNatureOfSignalData();
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

        this._natureOfSignalService.importNatureOfSignal(selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getNatureOfSignalData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<NatureOfSignalExportDtoModel>("NatureOfSignal", res.records, importNatureOfSignalTypeColumns);
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