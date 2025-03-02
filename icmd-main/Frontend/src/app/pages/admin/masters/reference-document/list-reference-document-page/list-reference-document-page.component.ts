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
import { DocumentTypeService } from 'src/app/service/documentType';
import {
  ReferenceDocumentDialogsService,
  ReferenceDocumentSearchHelperService,
  ReferenceDocumentService
} from 'src/app/service/reference-document';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import {
  ListReferenceDocumentTableComponent,
  ReferenceDocumentInfoDtoModel
} from '@c/masters/reference-document/list-reference-document-table';
import { RefBulkDialogComponent } from '@c/shared/bulkDelete-dialog/project-master/ref-master/ref-bulk-dialog.component';
import {
  FormBaseComponent,
  FormDefaultsModule
} from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import {
  CustomFieldSearchModel,
  DropdownInfoDtoModel
} from '@m/common';
import {
  importReferenceDocumentColumns,
  masterReferenceDocumentListTableColumn
} from '@u/constants';
import { listColumnMemoryCacheKey } from '@u/default';
import { getGroup } from '@u/forms';
import { ExcelHelper } from '@u/helper';

import { SearchReferenceDocumentFilterModel } from './list-reference-document-page.model';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-reference-document-page",
    templateUrl: "./list-reference-document-page.component.html",
    imports: [
        CommonModule,
        FormsModule,
        MatDatepickerModule,
        MatNativeDateModule,
        FormDefaultsModule,
        ListReferenceDocumentTableComponent,
        MatDialogModule,
        MatExpansionModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        ReferenceDocumentService,
        ReferenceDocumentSearchHelperService,
        ReferenceDocumentDialogsService,
        DialogsService,
        DocumentTypeService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListReferenceDocumentPageComponent extends FormBaseComponent<SearchReferenceDocumentFilterModel> {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListReferenceDocumentTableComponent) referenceDocumentTable: ListReferenceDocumentTableComponent;
    protected projectId: string = null;
    protected documentType: DropdownInfoDtoModel[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected referenceDocumentListColumns = [...masterReferenceDocumentListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _referenceDocumentSearchHelperService: ReferenceDocumentSearchHelperService,
        private _referenceDocumentService: ReferenceDocumentService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        private _referenceDocumentDialogService: ReferenceDocumentDialogsService,
        private _documentTypeService: DocumentTypeService,
        protected appConfig: AppConfig, private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        super(
            getGroup<SearchReferenceDocumentFilterModel>({
                referenceDocumentTypeId: {}
            })
        )
        this.getAllDocumentTypeInfo();
        this.getReferenceDocumentData();
    }

    ngAfterViewInit(): void {
        this.referenceDocumentTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._referenceDocumentSearchHelperService.updateSortingChange(res);
        });

        this.referenceDocumentTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._referenceDocumentSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._referenceDocumentSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getReferenceDocumentData();
                this.getMemoryCacheItem();
            }
        });

        this.referenceDocumentTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._referenceDocumentSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this reference-document?",
            "Confirm"
        );
        if (isOk) {
            this._referenceDocumentService.deleteReferenceDocument($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getReferenceDocumentData();
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
    protected async deleteBulkRef(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(RefBulkDialogComponent, {
            width: "600",
            data: ids
        });

        dialogRef.afterClosed().subscribe((result: string[] | null) => {
            if (result) {
                this._referenceDocumentService.deleteBulkReferenceDocument(result).pipe(takeUntil(this.destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                            this.getReferenceDocumentData();
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

    protected async addEditReferenceDocumentDialog(event: string = null): Promise<void> {
        await this._referenceDocumentDialogService.openReferenceDocumentDialog(event, this.projectId, this.documentType);
        this.getReferenceDocumentData();
    }

    protected async bulkEditReferenceDocument(): Promise<void> {
        const fileName = "Edit_ReferenceDocument"

        this.defaultCustomFilter(true, this.columnFilterList);
        this._referenceDocumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping: Record<string, string> = [{ key: 'id', label: 'Id' }]
                .concat(masterReferenceDocumentListTableColumn.filter(({ key }) => key !== 'actions'))
                .reduce((acc, { key, label }) => {
                    acc[`${key}`] = `${label}`;
                    return acc;
                }, {} as Record<string, string>);

                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected resetFilter() {
        this.form.reset();
        this.defaultCustomFilter();
    }

    protected exportData(): void {
        const fileName = 'Export_Reference_Document';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._referenceDocumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.referenceDocumentListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.referenceDocumentListColumns, listColumnMemoryCacheKey.referenceDocument);
        let selectedColumn = masterReferenceDocumentListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.referenceDocumentTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getReferenceDocumentData();
        }
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;
        if (formValue.referenceDocumentTypeId != null && formValue.referenceDocumentTypeId.length != 0) {
            filters.push({
                fieldName: "referenceDocumentTypeId",
                fieldValue: formValue.referenceDocumentTypeId?.join(","),
                searchType: SearchType.Contains, isColumnFilter: false
            });
        }
        filters.push(
            { fieldName: "projectIds", fieldValue: this.projectId, searchType: SearchType.Contains, isColumnFilter: false },
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        )

        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)
        this.customFilters$.next(filters);
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.referenceDocumentTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.referenceDocumentTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.referenceDocumentTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.referenceDocument).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.referenceDocumentTable.displayedColumns = [...this.selectedColumns, masterReferenceDocumentListTableColumn[masterReferenceDocumentListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.referenceDocumentListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    private getReferenceDocumentData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._referenceDocumentSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    private getAllDocumentTypeInfo(): void {
        this._documentTypeService.getAllDocumentTypeInfo()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.documentType = res;
            })
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_ReferenceDocument', columnHeaders: importReferenceDocumentColumns, fieldSeparator: "," });
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

        this._referenceDocumentService.validateImportRefDocument(this.projectId, selectedFile)
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
                            this.getReferenceDocumentData();
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

        this._referenceDocumentService.importReferenceDocument(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getReferenceDocumentData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<ReferenceDocumentInfoDtoModel>("Ref Document", res.records, importReferenceDocumentColumns);
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