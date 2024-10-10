import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { DeviceTypeListDtoModel, ListDeviceTypeTableComponent } from "@c/masters/device-type/list-device-type-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importDeviceTypeColumns, masterDeviceTypeListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { DeviceTypeDialogsService, DeviceTypeSearchHelperService, DeviceTypeService } from "src/app/service/device-type";

@Component({
    standalone: true,
    selector: "app-list-device-type-page",
    templateUrl: "./list-device-type-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListDeviceTypeTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        DeviceTypeService,
        DeviceTypeSearchHelperService,
        DialogsService,
        ExcelHelper,
        DeviceTypeDialogsService, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListDeviceTypePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListDeviceTypeTableComponent) deviceTypeTable: ListDeviceTypeTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected deviceTypeListColumns = [...masterDeviceTypeListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _deviceTypeSearchHelperService: DeviceTypeSearchHelperService,
        private _deviceTypeService: DeviceTypeService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _deviceTypeDialogService: DeviceTypeDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getDeviceTypeData();
    }

    ngAfterViewInit(): void {
        this.deviceTypeTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._deviceTypeSearchHelperService.updateSortingChange(res);
        });

        this.deviceTypeTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._deviceTypeSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._deviceTypeSearchHelperService.updateFilterChange(filter);
        });
        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._deviceTypeSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this type?",
            "Confirm"
        );
        if (isOk) {
            this._deviceTypeService.deleteDeviceType($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getDeviceTypeData();
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

    protected async addEditTypeDialog(event: string = null): Promise<void> {
        await this._deviceTypeDialogService.openDeviceTypeDialog(event);
        this.getDeviceTypeData();
    }

    protected exportData(): void {
        const fileName = 'Export_Device_Types';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._deviceTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.deviceTypeListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.deviceTypeListColumns, listColumnMemoryCacheKey.deviceType);
        let selectedColumn = masterDeviceTypeListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.deviceTypeTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getDeviceTypeData();
        }
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.deviceType).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.deviceTypeTable.displayedColumns = [...this.selectedColumns, masterDeviceTypeListTableColumn[masterDeviceTypeListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.deviceTypeListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    private getDeviceTypeData(): void {
        this.defaultCustomFilter();
        this._deviceTypeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.deviceTypeTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
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
        const csvConfig = mkConfig({ filename: 'Sample_DeviceType', columnHeaders: importDeviceTypeColumns, fieldSeparator: "," });
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

        this._deviceTypeService.importDeviceType(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getDeviceTypeData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<DeviceTypeListDtoModel>("DeviceType", res.records, importDeviceTypeColumns);

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