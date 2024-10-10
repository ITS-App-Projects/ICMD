import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { ListSystemTableComponent, SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { CustomFieldSearchModel } from "@m/common";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { CommonService, DialogsService } from "src/app/service/common";
import { SystemDialogsService, SystemSearchHelperService, SystemService } from "src/app/service/system";
import { SearchSystemFilterModel } from "./list-system-page.model";
import { getGroup } from "@u/forms";
import { WorkAreaPackService } from "src/app/service/workAreaPack";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";
import { MatExpansionModule } from "@angular/material/expansion";
import { ExcelHelper } from "@u/helper";
import { SearchType } from "@e/common";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { importSystemColumns, masterSystemListTableColumn } from "@u/constants";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { listColumnMemoryCacheKey } from "@u/default";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ListActionsComponent } from "@c/shared/list-actions";

@Component({
    standalone: true,
    selector: "app-list-system-page",
    templateUrl: "./list-system-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListSystemTableComponent,
        MatExpansionModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        SystemService,
        SystemSearchHelperService,
        DialogsService,
        WorkAreaPackService,
        SystemDialogsService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListSystemPageComponent extends FormBaseComponent<SearchSystemFilterModel> {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListSystemTableComponent) systemTable: ListSystemTableComponent;
    protected workAreaPackData: WorkAreaPackInfoDtoModel[] = [];
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected systemListColumns = [...masterSystemListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _systemSearchHelperService: SystemSearchHelperService,
        private _systemService: SystemService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _workAreaPackService: WorkAreaPackService,
        private _systemDialogService: SystemDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        super(
            getGroup<SearchSystemFilterModel>({
                workAreaPackId: {}
            })
        );
        this.getSystemData();
    }

    ngAfterViewInit(): void {
        this.systemTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._systemSearchHelperService.updateSortingChange(res);
        });

        this.systemTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._systemSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._systemSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getAllWorkAreaPackInfo();
                this.getSystemData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._systemSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this system?",
            "Confirm"
        );
        if (isOk) {
            this._systemService.deleteSystem($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSystemData();
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

    protected async addEditSystemDialog(event: string = null): Promise<void> {
        await this._systemDialogService.openSystemDialog(event, this.workAreaPackData);
        this.getSystemData();
    }

    protected resetFilter() {
        this.form.reset();
        this.defaultCustomFilter();
    }

    protected exportData(): void {
        const fileName = 'Export_Systems';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._systemSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.systemListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.systemListColumns, listColumnMemoryCacheKey.systems);
        let selectedColumn = masterSystemListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.systemTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getSystemData();
        }
    }


    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;
        if (formValue.workAreaPackId != null && formValue.workAreaPackId.length != 0) {
            filters.push({
                fieldName: "workAreaPackId",
                fieldValue: formValue.workAreaPackId?.join(","),
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
        combineLatest(this.systemTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.systems).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.systemTable.displayedColumns = [...this.selectedColumns, masterSystemListTableColumn[masterSystemListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.systemListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    private getSystemData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._systemSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    private getAllWorkAreaPackInfo(): void {
        if (this.projectId) {
            this._workAreaPackService.getAllWorkAreaPackInfo(this.projectId).subscribe((res) => {
                this.workAreaPackData = res;
            })
        }

    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_System', columnHeaders: importSystemColumns, fieldSeparator: "," });
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
        this._systemService.importSystem(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSystemData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<SystemInfoDtoModel>("System", res.records, importSystemColumns);

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