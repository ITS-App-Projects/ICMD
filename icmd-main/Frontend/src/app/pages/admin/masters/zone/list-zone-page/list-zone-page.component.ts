import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { ListZoneTableComponent, ZoneInfoDtoModel } from "@c/masters/zone/list-zone-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importZoneFileColumns, masterZoneListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { ZoneDialogsService, ZoneSearchHelperService, ZoneService } from "src/app/service/zone";

@Component({
    standalone: true,
    selector: "app-list-zone-page",
    templateUrl: "./list-zone-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListZoneTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        ZoneService,
        ZoneSearchHelperService,
        DialogsService,
        ZoneDialogsService,
        ExcelHelper,
        CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListZonePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListZoneTableComponent) zoneTable: ListZoneTableComponent;
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected zoneListColumns = [...masterZoneListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _zoneSearchHelperService: ZoneSearchHelperService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _zoneService: ZoneService,
        private _zoneDialogService: ZoneDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper, private _commonService: CommonService, private _cd: ChangeDetectorRef,
        private _columnSelectorDialogService: ColumnSelectorDialogsService) {
        this.getZoneData();
    }

    ngAfterViewInit(): void {
        this.zoneTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._zoneSearchHelperService.updateSortingChange(res);
        });

        this.zoneTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._zoneSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._zoneSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getZoneData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._zoneSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this zone?",
            "Confirm"
        );
        if (isOk) {
            this._zoneService.deleteZone($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getZoneData();
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

    protected async addEditZoneDialog(event: string = null): Promise<void> {
        await this._zoneDialogService.openZoneDialog(event, this.projectId);
        this.getZoneData();
    }

    protected exportData(): void {
        const fileName = 'Export_Zones';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._zoneSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.zoneListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.zoneListColumns, listColumnMemoryCacheKey.zones);
        let selectedColumn = masterZoneListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.zoneTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getZoneData();
        }
    }

    private getZoneData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();

            this._zoneSearchHelperService
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
    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.zoneTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.zones).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.zoneTable.displayedColumns = [...this.selectedColumns, masterZoneListTableColumn[masterZoneListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.zoneListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

     //#region Import Functionality
     protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Zone', columnHeaders: importZoneFileColumns, fieldSeparator: "," });
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

        this._zoneService.importZone(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getZoneData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<ZoneInfoDtoModel>("Zone", res.records, importZoneFileColumns);

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