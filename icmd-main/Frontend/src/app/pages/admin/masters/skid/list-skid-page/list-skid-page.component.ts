import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { JunctionBoxListDtoModel, ListJunctionBoxTableComponent } from "@c/masters/junction-box/list-junction-box-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { RecordType, SearchType } from "@e/common";
import { ActiveInActiveDtoModel, CustomFieldSearchModel } from "@m/common";
import { SearchProjectFilterModel } from "@p/admin/manage-projects/list-project-page";
import { importSkidColumns, masterJunctionBoxListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { getGroup } from "@u/forms";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { SkidDialogsService, SkidSearchHelperService, SkidService } from "src/app/service/skid";

@Component({
    standalone: true,
    selector: "app-list-skid-page",
    templateUrl: "./list-skid-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListJunctionBoxTableComponent,
        MatDialogModule,
        MatExpansionModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        SkidService,
        SkidSearchHelperService,
        DialogsService,
        ExcelHelper,
        SkidDialogsService, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListSkidPageComponent extends FormBaseComponent<SearchProjectFilterModel> {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListJunctionBoxTableComponent) skidTable: ListJunctionBoxTableComponent;
    protected projectId: string = null;
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected skidListColumns = [...masterJunctionBoxListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _skidSearchHelperService: SkidSearchHelperService,
        private _skidService: SkidService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        protected appConfig: AppConfig,
        private _skidDialogService: SkidDialogsService,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        super(
            getGroup<SearchProjectFilterModel>({
                type: { v: RecordType.Active }
            })
        );
        const keys = Object.keys(this.recordTypeEnum);
        this.recordType = keys.slice(keys.length / 2);
        this.getSkidData();
    }

    ngAfterViewInit(): void {
        this.skidTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._skidSearchHelperService.updateSortingChange(res);
        });

        this.skidTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._skidSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._skidSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getSkidData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._skidSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this skid?",
            "Confirm"
        );
        if (isOk) {
            this._skidService.deleteSkid($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSkidData();
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

    protected async addEditSkidDialog(event: string = null): Promise<void> {
        await this._skidDialogService.openSkidDialog(event, this.projectId);
        this.getSkidData();
    }

    protected resetFilter() {
        this.form.reset();
        this.field('type').setValue(RecordType.Active);
        this.defaultCustomFilter();
    }

    protected exportData(): void {
        const fileName = 'Export_Skids';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._skidSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.skidListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    private getSkidData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._skidSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    protected async activeInactiveSkid($event: ActiveInActiveDtoModel): Promise<void> {
        const msg = !$event.isActive
            ? 'Are you sure you want to activate this skid?'
            : 'Are you sure you want to deactivate this skid?';
        const isOk = await this._dialog.confirm(
            msg,
            'Confirm'
        );
        if (isOk) {
            $event.isActive = !$event.isActive;
            this._skidService.activeInActiveSkid($event).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSkidData();
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

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;
        if (formValue.type != null) {
            filters.push({
                fieldName: "type",
                fieldValue: formValue.type?.toString(),
                searchType: SearchType.Contains, isColumnFilter: false
            });
        }
        filters.push(
            { fieldName: "projectIds", fieldValue: this.projectId, searchType: SearchType.Contains, isColumnFilter: false },
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        );

        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)
        this.customFilters$.next(filters);
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.skidListColumns, listColumnMemoryCacheKey.skid);
        let selectedColumn = masterJunctionBoxListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.skidTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getSkidData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.skidTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.skid).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.skidTable.displayedColumns = [...this.selectedColumns, masterJunctionBoxListTableColumn[masterJunctionBoxListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.skidListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality 
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Skid', columnHeaders: importSkidColumns, fieldSeparator: "," });
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

        this._skidService.importSkid(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getSkidData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<JunctionBoxListDtoModel>("skid", res.records, importSkidColumns);

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