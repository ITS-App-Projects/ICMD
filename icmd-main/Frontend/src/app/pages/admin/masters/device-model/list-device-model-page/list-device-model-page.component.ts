import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { DeviceModelListDtoModel, ListDeviceModelTableComponent } from "@c/masters/device-model/list-device-model-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel, DropdownInfoDtoModel } from "@m/common";
import { importDeviceModelColumns, masterDeviceListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { DeviceModelDialogsService, DeviceModelSearchHelperService, DeviceModelService } from "src/app/service/device-model";
import { ManufacturerService } from "src/app/service/manufacturer";

@Component({
    standalone: true,
    selector: "app-list-device-model-page",
    templateUrl: "./list-device-model-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListDeviceModelTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        DeviceModelService,
        DeviceModelSearchHelperService,
        DialogsService,
        ExcelHelper,
        ManufacturerService,
        DeviceModelDialogsService, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListDeviceModelPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListDeviceModelTableComponent) deviceModelTable: ListDeviceModelTableComponent;
    protected manufacturerData: DropdownInfoDtoModel[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected deviceListColumns = [...masterDeviceListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _deviceModelSearchHelperService: DeviceModelSearchHelperService,
        private _deviceModelService: DeviceModelService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _manufacturerService: ManufacturerService,
        private _deviceModelDialogServic: DeviceModelDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getAllManufacturerData();
        this.getDeviceModelData();
    }

    ngAfterViewInit(): void {
        this.deviceModelTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._deviceModelSearchHelperService.updateSortingChange(res);
        });

        this.deviceModelTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._deviceModelSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._deviceModelSearchHelperService.updateFilterChange(filter);
        });
        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._deviceModelSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this model?",
            "Confirm"
        );
        if (isOk) {
            this._deviceModelService.deleteDeviceModel($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getDeviceModelData();
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

    protected async addEditModelDialog(event: string = null): Promise<void> {
        await this._deviceModelDialogServic.openDeviceModelDialog(event, this.manufacturerData);
        this.getDeviceModelData();
    }

    protected exportData(): void {
        const fileName = 'Export_Device_Models';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._deviceModelSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.deviceListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.deviceListColumns, listColumnMemoryCacheKey.deviceModel);
        let selectedColumn = masterDeviceListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.deviceModelTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getDeviceModelData();
        }
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.deviceModel).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.deviceModelTable.displayedColumns = [...this.selectedColumns, masterDeviceListTableColumn[masterDeviceListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.deviceListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    private getDeviceModelData(): void {
        this.defaultCustomFilter();
        this._deviceModelSearchHelperService
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

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.deviceModelTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getAllManufacturerData(): void {
        this._manufacturerService.getAllManufacturerInfo()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res => {
                this.manufacturerData = res;
            }))
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_DeviceModel', columnHeaders: importDeviceModelColumns, fieldSeparator: "," });
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

        this._deviceModelService.importDeviceModel(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getDeviceModelData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<DeviceModelListDtoModel>("DeviceModel", res.records, importDeviceModelColumns);

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