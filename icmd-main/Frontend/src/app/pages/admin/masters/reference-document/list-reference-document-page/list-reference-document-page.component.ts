import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { ListReferenceDocumentTableComponent, ReferenceDocumentInfoDtoModel } from "@c/masters/reference-document/list-reference-document-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { CustomFieldSearchModel, DropdownInfoDtoModel } from "@m/common";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { CommonService, DialogsService } from "src/app/service/common";
import { DocumentTypeService } from "src/app/service/documentType";
import { ReferenceDocumentDialogsService, ReferenceDocumentSearchHelperService, ReferenceDocumentService } from "src/app/service/reference-document";
import { SearchReferenceDocumentFilterModel } from "./list-reference-document-page.model";
import { getGroup } from "@u/forms";
import { FormsModule } from "@angular/forms";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatNativeDateModule } from "@angular/material/core";
import { ExcelHelper } from "@u/helper";
import { SearchType } from "@e/common";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { importReferenceDocumentColumns, masterReferenceDocumentListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ListActionsComponent } from "@c/shared/list-actions";

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

    constructor(
        protected _referenceDocumentSearchHelperService: ReferenceDocumentSearchHelperService,
        private _referenceDocumentService: ReferenceDocumentService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
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
        })
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

    protected async addEditReferenceDocumentDialog(event: string = null): Promise<void> {
        await this._referenceDocumentDialogService.openReferenceDocumentDialog(event, this.projectId, this.documentType);
        this.getReferenceDocumentData();
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
        this.columnFilterList = [];
        combineLatest(this.referenceDocumentTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
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

        this._referenceDocumentService.importReferenceDocument(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getReferenceDocumentData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<ReferenceDocumentInfoDtoModel>("Reference Document", res.records, importReferenceDocumentColumns);

                },
                error: (errorRes) => {
                    this.clearFileInput();
                    if (errorRes?.error?.message) {
                        this._toastr.error(errorRes?.error?.message);
                    }
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