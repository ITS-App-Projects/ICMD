import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { ListSubSystemTableComponent, SubSystemInfoDtoModel } from "@c/masters/sub-system/list-sub-system-table";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { CustomFieldSearchModel } from "@m/common";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { CommonService, DialogsService } from "src/app/service/common";
import { SubSystemDialogsService, SubSystemSearchHelperService, SubSystemService } from "src/app/service/sub-system";
import { SystemService } from "src/app/service/system";
import { WorkAreaPackService } from "src/app/service/workAreaPack";
import { SearchSubSystemFilterModel } from "./list-sub-system-page.model";
import { getGroup } from "@u/forms";
import { ExcelHelper } from "@u/helper";
import { SearchType } from "@e/common";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { importSubSystemColumns, masterSubSystemListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ListActionsComponent } from "@c/shared/list-actions";

@Component({
    standalone: true,
    selector: "app-list-sub-system-page",
    templateUrl: "./list-sub-system-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        MatExpansionModule,
        ListSubSystemTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        SubSystemService,
        SubSystemSearchHelperService,
        DialogsService,
        SystemService,
        SubSystemDialogsService,
        WorkAreaPackService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListSubSystemPageComponent extends FormBaseComponent<SearchSubSystemFilterModel> {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListSubSystemTableComponent) subSystemTable: ListSubSystemTableComponent;
    protected projectId: string = null;
    protected workAreaPackData: WorkAreaPackInfoDtoModel[] = [];
    protected systemData: SystemInfoDtoModel[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected subSystemListColumns = [...masterSubSystemListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _subSystemSearchHelperService: SubSystemSearchHelperService,
        private _subSystemService: SubSystemService,
        private _systemService: SystemService,
        private _workAreaPackService: WorkAreaPackService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _subSystemDialogService: SubSystemDialogsService,
        private _excelHelper: ExcelHelper,
        protected appConfig: AppConfig,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        super(
            getGroup<SearchSubSystemFilterModel>({
                workAreaPackId: {},
                systemId: {}
            })
        )
        this.getSubSystemData();
    }

    ngAfterViewInit(): void {
        this.subSystemTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._subSystemSearchHelperService.updateSortingChange(res);
        });

        this.subSystemTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._subSystemSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._subSystemSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getAllSystemAndWorkAreaInfo();
                this.getSubSystemData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._subSystemSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this sub-system?",
            "Confirm"
        );
        if (isOk) {
            this._subSystemService.deleteSubSystem($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSubSystemData();
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

    protected async addEditSubSystemDialog(event: string = null): Promise<void> {
        await this._subSystemDialogService.openSubSystemDialog(event, this.systemData);
        this.getSubSystemData();
    }


    protected resetFilter() {
        this.form.reset();
        this.defaultCustomFilter();
    }

    protected exportData(): void {
        const fileName = 'Export_Sub_Systems';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._subSystemSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.subSystemListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.subSystemListColumns, listColumnMemoryCacheKey.subSystem);
        let selectedColumn = masterSubSystemListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.subSystemTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getSubSystemData();
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

        //System
        if (formValue.systemId != null && formValue.systemId.length != 0) {
            filters.push({
                fieldName: "systemId",
                fieldValue: formValue.systemId?.join(","),
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
        combineLatest(this.subSystemTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getSubSystemData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._subSystemSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    private getAllSystemAndWorkAreaInfo(): void {
        if (this.projectId) {
            combineLatest([
                this._workAreaPackService.getAllWorkAreaPackInfo(this.projectId),
                this._systemService.getAllSystemInfo(this.projectId)
            ]).pipe(takeUntil(this._destroy$)).subscribe(([workAreaPacks, systems]) => {
                this.workAreaPackData = workAreaPacks;
                this.systemData = systems;
            });
        }
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.subSystem).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.subSystemTable.displayedColumns = [...this.selectedColumns, masterSubSystemListTableColumn[masterSubSystemListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.subSystemListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_SubSystem', columnHeaders: importSubSystemColumns, fieldSeparator: "," });
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

        this._subSystemService.importSubSystem(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSubSystemData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<SubSystemInfoDtoModel>("SubSystem", res.records, importSubSystemColumns);

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