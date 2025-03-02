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
  DocumentTypeDialogsService,
  DocumentTypeSearchHelperService,
  DocumentTypeService
} from 'src/app/service/documentType';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  ListDocumentTypeTableComponent,
  TypeInfoDtoModel
} from '@c/masters/documentType/list-document-type-table';
import { DocumentTypeBulkDialogComponent } from '@c/shared/bulkDelete-dialog/system-master/reference-document-type/documentType-bulk-dialog.component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import { CustomFieldSearchModel } from '@m/common';
import { importReferenceDocumentType } from '@u/constants';
import { ExcelHelper } from '@u/helper';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-document-type-page",
    templateUrl: "./list-document-type-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListDocumentTypeTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        DocumentTypeService,
        DocumentTypeSearchHelperService,
        DialogsService,
        DocumentTypeDialogsService,
        ExcelHelper
    ]
})
export class ListDocumentTypePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListDocumentTypeTableComponent) documentTypeTable: ListDocumentTypeTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _documentTypeSearchHelperService: DocumentTypeSearchHelperService,
        private _documentTypeService: DocumentTypeService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        private _documentTypeDialogsService: DocumentTypeDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getDocumentTypeData();
    }

    ngAfterViewInit(): void {
        this.documentTypeTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._documentTypeSearchHelperService.updateSortingChange(res);
        });

        this.documentTypeTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._documentTypeSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._documentTypeSearchHelperService.updateFilterChange(filter);
        });

        this.documentTypeTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });

        this.tableColumnchanges();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._documentTypeSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this document type?",
            "Confirm"
        );
        if (isOk) {
            this._documentTypeService.deleteDocumentType($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getDocumentTypeData();
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
        const dialogRef = this.dialog.open(DocumentTypeBulkDialogComponent, {
            width: "600",
            data: ids
        });

        dialogRef.afterClosed().subscribe((result: string[] | null) => {
            if (result) {
                this._documentTypeService.deleteBulkDocumentType(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            if (res.isWarning)
                                this._toastr.warning(res.message);
                            else
                                this._toastr.success(res.message);

                            this.getDocumentTypeData();
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

    protected async bulkEdit(): Promise<void> {
        const fileName = 'Edit_DocumentType';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._documentTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'id' : 'Id',
                    'type': 'Type',
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async addEditDocumentTypeDialog(event: string = null): Promise<void> {
        await this._documentTypeDialogsService.openDocumentTypeDialog(event);
        this.getDocumentTypeData();
    }

    protected exportData(): void {
        const fileName = 'Export_Reference_DocumentType';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._documentTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = {
                    'type': 'Type',
                };
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.documentTypeTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.documentTypeTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.documentTypeTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    private getDocumentTypeData(): void {
        this.defaultCustomFilter();
        this._documentTypeSearchHelperService
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

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Reference_Document_Type', columnHeaders: importReferenceDocumentType, fieldSeparator: "," });
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

        this._documentTypeService.validateImportReferenceDocumentType(selectedFile)
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
                            this.getDocumentTypeData();
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

        this._documentTypeService.importReferenceDocumentType(selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getDocumentTypeData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<TypeInfoDtoModel>("ReferenceDocumentType", res.records, importReferenceDocumentType);
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