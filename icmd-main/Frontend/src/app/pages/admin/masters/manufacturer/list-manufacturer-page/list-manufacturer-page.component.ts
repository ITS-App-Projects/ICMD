import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { ListManufacturerTableComponent, ManufacturerInfoDtoModel } from "@c/masters/manufacturer/list-manufacturer-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importManufacturerColumns, masterManufacturerListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { ManufacturerDialogsService, ManufacturerSearchHelperService, ManufacturerService } from "src/app/service/manufacturer";

@Component({
    standalone: true,
    selector: "app-list-manufacturer-page",
    templateUrl: "./list-manufacturer-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListManufacturerTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        ManufacturerService,
        ManufacturerSearchHelperService,
        ManufacturerDialogsService,
        DialogsService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListManufacturerPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListManufacturerTableComponent) manufacturerTable: ListManufacturerTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected manufacturerListColumns = [...masterManufacturerListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _manufacturerSearchHelperService: ManufacturerSearchHelperService,
        private _manufacturerService: ManufacturerService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        protected appConfig: AppConfig,
        private _manufacturerDialogsService: ManufacturerDialogsService,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getManufacturerData();
    }

    ngAfterViewInit(): void {
        this.manufacturerTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._manufacturerSearchHelperService.updateSortingChange(res);
        });

        this.manufacturerTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._manufacturerSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._manufacturerSearchHelperService.updateFilterChange(filter);
        });

        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._manufacturerSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this manufacturer?",
            "Confirm"
        );
        if (isOk) {
            this._manufacturerService.deleteManufacturer($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getManufacturerData();
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

    protected async addEditManufacturerDialog(event: string = null): Promise<void> {
        await this._manufacturerDialogsService.openManufacturerDialog(event);
        this.getManufacturerData();
    }

    protected exportData(): void {
        const fileName = 'Export_Manufacturer';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._manufacturerSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.manufacturerListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.manufacturerListColumns, listColumnMemoryCacheKey.manufacturer);
        let selectedColumn = masterManufacturerListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.manufacturerTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getManufacturerData();
        }
    }

    private getManufacturerData(): void {
        this.defaultCustomFilter();
        this._manufacturerSearchHelperService
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
        combineLatest(this.manufacturerTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.manufacturer).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.manufacturerTable.displayedColumns = [...this.selectedColumns, masterManufacturerListTableColumn[masterManufacturerListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.manufacturerListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Manufacturer', columnHeaders: importManufacturerColumns, fieldSeparator: "," });
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
        
        this._manufacturerService.importManufacturer(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getManufacturerData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<ManufacturerInfoDtoModel>("Manufacturer", res.records, importManufacturerColumns);

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