import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { ListSubProcessTableComponent, SubProcessInfoDtoModel } from "@c/masters/sub-process/list-sub-process-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importTagField2Columns, masterSubProcessListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { mkConfig, generateCsv, download } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { SubProcessDialogsService, SubProcessSearchHelperService, SubProcessService } from "src/app/service/sub-process";

@Component({
    standalone: true,
    selector: "app-list-sub-process-page",
    templateUrl: "./list-sub-process-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListSubProcessTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        SubProcessService,
        SubProcessSearchHelperService,
        DialogsService,
        SubProcessDialogsService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListSubProcessPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListSubProcessTableComponent) subProcessTable: ListSubProcessTableComponent;
    protected projectId: string = null;
    protected title: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected subProcessListColumns = [...masterSubProcessListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _subProcessSearchHelperService: SubProcessSearchHelperService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _subProcessService: SubProcessService,
        private _subProcessDialogService: SubProcessDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getSubProcessData();
    }

    ngAfterViewInit(): void {
        this.subProcessTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._subProcessSearchHelperService.updateSortingChange(res);
        });

        this.subProcessTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._subProcessSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._subProcessSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getSubProcessData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._subProcessSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this sub-process?",
            "Confirm"
        );
        if (isOk) {
            this._subProcessService.deleteSubProcess($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSubProcessData();
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

    protected async addEditSubProcessDialog(event: string = null): Promise<void> {
        await this._subProcessDialogService.openSubProcessDialog(event, this.projectId);
        this.getSubProcessData();
    }

    protected exportData(): void {
        const fileName = 'Export_Sub_Process';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._subProcessSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.subProcessListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.subProcessListColumns, listColumnMemoryCacheKey.tagField2);
        let selectedColumn = masterSubProcessListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.subProcessTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getSubProcessData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.subProcessTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getSubProcessData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._subProcessSearchHelperService
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
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.tagField2).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.subProcessTable.displayedColumns = [...this.selectedColumns, masterSubProcessListTableColumn[masterSubProcessListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.subProcessListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_TagField2', columnHeaders: importTagField2Columns, fieldSeparator: "," });
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

        this._subProcessService.importSubProcess(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSubProcessData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<SubProcessInfoDtoModel>("TagField2", res.records, importTagField2Columns);

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