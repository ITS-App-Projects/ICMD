import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { BankInfoDtoModel } from "@c/masters/bank/list-bank-table";
import { ListWorkAreaTableComponent, WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importBankFileColumns, importWorkAreaPackFileColumns, masterWorkAreaListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { mkConfig, generateCsv, download } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { ProjectService } from "src/app/service/manage-projects";
import { WorkAreaPackDialogsService, WorkAreaPackSearchHelperService, WorkAreaPackService } from "src/app/service/workAreaPack";

@Component({
    standalone: true,
    selector: "app-list-work-area-page",
    templateUrl: "./list-work-area-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        MatExpansionModule,
        ListWorkAreaTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        WorkAreaPackService,
        WorkAreaPackSearchHelperService,
        WorkAreaPackDialogsService,
        DialogsService,
        ProjectService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListWorkAreaPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListWorkAreaTableComponent) workAreaTable: ListWorkAreaTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    protected projectId: string = null;
    private _destroy$ = new Subject<void>();
    protected workAreaListColumns = [...masterWorkAreaListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _workAreaPackSearchHelperService: WorkAreaPackSearchHelperService,
        private _workAreaPackService: WorkAreaPackService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _projectService: ProjectService,
        private _workAreaPackDialogService: WorkAreaPackDialogsService,
        private _excelHelper: ExcelHelper,
        protected appConfig: AppConfig,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {

        this.getWorkAreaPackData();
    }

    ngAfterViewInit(): void {
        this.workAreaTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._workAreaPackSearchHelperService.updateSortingChange(res);
        });

        this.workAreaTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._workAreaPackSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._workAreaPackSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getWorkAreaPackData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._workAreaPackSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this work area pack?",
            "Confirm"
        );
        if (isOk) {
            this._workAreaPackService.deleteWorkAreaPack($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getWorkAreaPackData();
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

    protected async addEditWorkAreaPackDialog(event: string = null): Promise<void> {
        await this._workAreaPackDialogService.openWorkAreaPackDialog(event, this.projectId);
        this.getWorkAreaPackData();
    }

    protected exportData(): void {
        const fileName = 'Export_WorkAreaPacks';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._workAreaPackSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.workAreaListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.workAreaListColumns, listColumnMemoryCacheKey.workAreaPack);
        let selectedColumn = masterWorkAreaListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.workAreaTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getWorkAreaPackData();
        }
    }

    private getWorkAreaPackData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._workAreaPackSearchHelperService
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

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.workAreaPack).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.workAreaTable.displayedColumns = [...this.selectedColumns, masterWorkAreaListTableColumn[masterWorkAreaListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.workAreaListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.workAreaTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_WorkAreaPack', columnHeaders: importWorkAreaPackFileColumns, fieldSeparator: "," });
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

        this._workAreaPackService.importWorkAreaPack(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getWorkAreaPackData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<WorkAreaPackInfoDtoModel>("WorkAreaPack", res.records, importWorkAreaPackFileColumns);

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