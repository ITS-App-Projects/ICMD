import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { EquipmentCodeInfoDtoModel, ListEquipmentCodeTableComponent } from "@c/masters/equipmentCode/list-equipmentCode-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importEquipmentCode, masterEquipmentCodeListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { EquipmentCodeDialogsService, EquipmentCodeSearchHelperService, EquipmentCodeService } from "src/app/service/equipmentCode";

@Component({
    standalone: true,
    selector: "app-list-equipmentCode-page",
    templateUrl: "./list-equipmentCode-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListEquipmentCodeTableComponent,
        MatDialogModule,
        MatExpansionModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        EquipmentCodeService,
        EquipmentCodeSearchHelperService,
        EquipmentCodeDialogsService,
        DialogsService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListEquipmentCodePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListEquipmentCodeTableComponent) equipmentCodeTable: ListEquipmentCodeTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected equipmentCodeListColumns = [...masterEquipmentCodeListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _equipmentCodeSearchHelperService: EquipmentCodeSearchHelperService,
        private _equipmentCodeService: EquipmentCodeService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        protected appConfig: AppConfig,
        private _equipmentCodeDialogsService: EquipmentCodeDialogsService,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getEquipmentCodeData();
    }

    ngAfterViewInit(): void {
        this.equipmentCodeTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._equipmentCodeSearchHelperService.updateSortingChange(res);
        });

        this.equipmentCodeTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._equipmentCodeSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._equipmentCodeSearchHelperService.updateFilterChange(filter);
        });

        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._equipmentCodeSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this equipment-code?",
            "Confirm"
        );
        if (isOk) {
            this._equipmentCodeService.deleteEquipmentCode($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getEquipmentCodeData();
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

    protected async addEditEquipmentCodeDialog(event: string = null): Promise<void> {
        await this._equipmentCodeDialogsService.openEquipmentCodeDialog(event);
        this.getEquipmentCodeData();
    }

    protected exportData(): void {
        const fileName = 'Export_Equipment_Code';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._equipmentCodeSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.equipmentCodeListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.equipmentCodeListColumns, listColumnMemoryCacheKey.equipmentCode);
        let selectedColumn = masterEquipmentCodeListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.equipmentCodeTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getEquipmentCodeData();
        }
    }

    private getEquipmentCodeData(): void {
        this.defaultCustomFilter();
        this._equipmentCodeSearchHelperService
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
        combineLatest(this.equipmentCodeTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.equipmentCode).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.equipmentCodeTable.displayedColumns = [...this.selectedColumns, masterEquipmentCodeListTableColumn[masterEquipmentCodeListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.equipmentCodeListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_EquipmentCode', columnHeaders: importEquipmentCode, fieldSeparator: "," });
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

        this._equipmentCodeService.importEquipmentCode(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getEquipmentCodeData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<EquipmentCodeInfoDtoModel>("EquipmentCode", res.records, importEquipmentCode);

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