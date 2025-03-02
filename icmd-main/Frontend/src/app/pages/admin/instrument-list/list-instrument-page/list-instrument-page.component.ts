import {
  download,
  generateCsv,
  mkConfig
} from 'export-to-csv';
import { ToastrService } from 'ngx-toastr';
import {
  combineLatest,
  forkJoin,
  BehaviorSubject,
  Observable,
  Subject
} from 'rxjs';
import {
  map,
  startWith,
  take,
  takeUntil
} from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import {
  ColumnSelectorDialogsService,
  ColumnTemplateService
} from 'src/app/service/column-selector';
import {
  CommonService,
  DialogsService
} from 'src/app/service/common';
import { DeviceService } from 'src/app/service/device';
import {
  InstrumentSearchHelperService,
  InstrumentService
} from 'src/app/service/instrument';
import { ProjectService } from 'src/app/service/manage-projects';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { ListInstrumentTableComponent } from '@c/instrument-list/list-instrument-table';
import { BulkDeleteDialogComponent } from '@c/shared/bulkDelete-dialog/instrument/bulk-delete-dialog.component';
import {
  FormBaseComponent,
  FormDefaultsModule
} from '@c/shared/forms';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import {
  RecordType,
  SearchType
} from '@e/common';
import {
  ActiveInActiveDtoModel,
  CustomFieldSearchModel,
  DropdownInfoDtoModel
} from '@m/common';
import { AppRoute } from '@u/app.route';
import { instrumentListTableColumns } from '@u/constants';
import { getGroup } from '@u/forms';
import { ExcelHelper } from '@u/helper';
import { listColumnMemoryCacheKey } from '@u/index';

import {
  InstrumentDropdownInfoDtoModel,
  SearchInstrumentFilterModel
} from './list-instrument-page.model';

@Component({
    standalone: true,
    selector: "app-list-instrument-page",
    templateUrl: "./list-instrument-page.component.html",
    imports: [
        MatDialogModule,
        CommonModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule,
        MatExpansionModule,
        ListInstrumentTableComponent,
        MatAutocompleteModule,
        PermissionWrapperComponent,
        MatCheckboxModule,
        ListActionsComponent

    ],
    providers: [DialogsService, InstrumentSearchHelperService, ProjectService, InstrumentService, ExcelHelper, DeviceService, CommonService, ColumnTemplateService, ColumnSelectorDialogsService]
})
export class ListInstrumentPageComponent extends FormBaseComponent<SearchInstrumentFilterModel> {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListInstrumentTableComponent) instrumentTable: ListInstrumentTableComponent;
    protected searchTypesEnum = SearchType;
    protected searchTypes: string[] = [];
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    protected equipmentCodeFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected manufacturerFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected natureOfSignalFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected processFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected plcNumberFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected tagFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected zoneFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected tagFieldNames: string[] = [];
    private projectId: string | null = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private instrumentDropdownData: InstrumentDropdownInfoDtoModel;
    private filterState: { [key: string]: any } = {};


    protected instrumentListColumns = [...instrumentListTableColumns.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(protected _instrumentSearchHelperService: InstrumentSearchHelperService,
        private _projectService: ProjectService,
        private _instrumentService: InstrumentService,
        private _dialog: DialogsService, private _toastr: ToastrService,
        private dialog: MatDialog,
        private _router: Router,
        private _deviceService: DeviceService,
        private _commonService: CommonService,
        private _columnTemplateService: ColumnTemplateService,
        protected appConfig: AppConfig, private _excelHelper: ExcelHelper, private _cd: ChangeDetectorRef,
        private _columnSelectorDialogService: ColumnSelectorDialogsService) {
        super(
            getGroup<SearchInstrumentFilterModel>({
                equipmentCode: {},
                equipmentCodeSearchType: { v: SearchType.Contains },
                manufacturer: {},
                manufacturerSearchType: { v: SearchType.Contains },
                natureOfSignal: {},
                natureOfSignalSearchType: { v: SearchType.Contains },

                process: {},
                processSearchType: { v: SearchType.Contains },
                plcNo: {},
                plcNoSearchType: { v: SearchType.Contains },
                tag: {},
                tagSearchType: { v: SearchType.Contains },
                zone: {},
                zoneSearchType: { v: SearchType.Contains },
                type: { v: RecordType.Active }
            })
        );
        const recordkeys = Object.keys(this.recordTypeEnum);
        this.recordType = recordkeys.slice(recordkeys.length / 2);
        const keys = Object.keys(this.searchTypesEnum);
        this.searchTypes = keys;
    }

    ngAfterViewInit(): void {
        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id;
                this.getProjectTagFieldName(this.projectId);
                this.getInstrumentData();
                this._instrumentSearchHelperService.updateProjectId(this.projectId);
                this.getInstrumentsDropdownInfo();
            }
        });

        this.instrumentTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._instrumentSearchHelperService.updateSortingChange(res);
        });

        this.instrumentTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._instrumentSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._instrumentSearchHelperService.updateFilterChange(filter);
        });

        this.instrumentTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });
    }

    protected showDevice(event: string = null): void {
        this.appConfig.isPreviousURL$ = AppRoute.instrumentList;
        this._router.navigate([AppRoute.manageDevice, event ?? ""]);
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._instrumentSearchHelperService.commonSearch($event);
    }

    protected resetFilter() {
        this.form.reset();
        this.field('tagSearchType').setValue(SearchType.Contains);
        this.field('plcNoSearchType').setValue(SearchType.Contains);
        this.field('equipmentCodeSearchType').setValue(SearchType.Contains);
        this.field('processSearchType').setValue(SearchType.Contains);
        this.field('manufacturerSearchType').setValue(SearchType.Contains);
        this.field('natureOfSignalSearchType').setValue(SearchType.Contains);
        this.field('zoneSearchType').setValue(SearchType.Contains);
        this.field('type').setValue(RecordType.Active);
        this.defaultCustomFilter();
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;

        if (formValue.tag != null && formValue.tag != "") {
            filters.push({
                fieldName: "tagName",
                fieldValue: formValue.tag?.toString(),
                searchType: formValue.tagSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.plcNo && formValue.plcNo != "") {
            filters.push({
                fieldName: "pLCNumber",
                fieldValue: formValue.plcNo?.toString(),
                searchType: formValue.plcNoSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.equipmentCode != null && formValue.equipmentCode != "") {
            filters.push({
                fieldName: "equipmentCode",
                fieldValue: formValue.equipmentCode?.toString(),
                searchType: formValue.equipmentCodeSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.process != null && formValue.process != "") {
            filters.push({
                fieldName: "processNo",
                fieldValue: formValue.process?.toString(),
                searchType: formValue.processSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.manufacturer != null && formValue.manufacturer != "") {
            filters.push({
                fieldName: "manufacturer",
                fieldValue: formValue.manufacturer?.toString(),
                searchType: formValue.manufacturerSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.natureOfSignal != null && formValue.natureOfSignal != "") {
            filters.push({
                fieldName: "natureOfSignal",
                fieldValue: formValue.natureOfSignal?.toString(),
                searchType: formValue.natureOfSignalSearchType,
                isColumnFilter: false
            });
        }

        if (formValue.zone != null && formValue.zone != "") {
            filters.push({
                fieldName: "zone",
                fieldValue: formValue.zone?.toString(),
                searchType: formValue.zoneSearchType,
                isColumnFilter: false
            });
        }
        if (formValue.type != null) {
            filters.push({
                fieldName: "type",
                fieldValue: formValue.type?.toString(),
                searchType: SearchType.Contains,
                isColumnFilter: false
            });
        }
        filters.push(
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        );

        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)

        this.customFilters$.next(filters);
    }

    protected exportData(): void {
        const fileName = 'Export_Instruments';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._instrumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.instrumentListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    //#region Delete
    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this device?",
            "Confirm"
        );
        if (isOk) {
            this._deviceService.deleteDevice($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getInstrumentData();
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

    //#region Delete Bulk
    protected async deleteBulk(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(BulkDeleteDialogComponent, {
            width: '600px',
            data: ids,
          });

          dialogRef.afterClosed().subscribe((result: string[] | null) => {

            if (result) {
                this._deviceService.deleteBulkInstrumentDevices(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                            this.getInstrumentData();
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes) => {
                        this._toastr.error(errorRes?.error?.message);
                    }
                );
            }
          });
    }

    protected async bulkEdit(): Promise<void> {
        const fileName = 'Edit_Instruments';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._instrumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping: Record<string, string> = [{ key: 'deviceId', label: 'Id' }]
                .concat(instrumentListTableColumns.filter(({ key }) => key !== 'actions'))
                .reduce((acc, { key, label }) => {
                    acc[`${key}`] = `${label}`;
                    return acc;
                }, {} as Record<string, string>);

                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected async activeInactiveStatus($event: ActiveInActiveDtoModel): Promise<void> {
        const msg = !$event.isActive
            ? 'Are you sure you want to activate this device?'
            : 'Are you sure you want to inactivate this device?';
        const isOk = await this._dialog.confirm(
            msg,
            'Confirm'
        );
        if (isOk) {
            $event.isActive = !$event.isActive;
            this._deviceService.activeInActiveDevice($event).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getInstrumentData();
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

    protected getProjectTagFieldName(projectId: string): void {
        if (projectId) {
            forkJoin([
                this._projectService.getProjectTagFieldNames(projectId),
                this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.instrumentColumn)
            ]).pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.tagFieldNames = [];
                    this.instrumentTable.tagFieldNames = [];
                    let array: string[] = [];
                    res[0].map((item, index) => {
                        array.push(item);
                    })
                    this.tagFieldNames = array;
                    this.instrumentTable.tagFieldNames = array;

                    this.instrumentTable.tagFieldNames.forEach((element: string, index: number) => {
                        this.instrumentListColumns[index].label = element;
                    });

                    const selectedColumn = res[1];
                    if (selectedColumn != null && selectedColumn.length > 0) {
                        this.selectedColumns = selectedColumn;
                        this.instrumentTable.displayedColumns = [...this.selectedColumns, instrumentListTableColumns[instrumentListTableColumns.length - 1].key];
                    } else {
                        this.selectedColumns = this.instrumentListColumns.map(x => x.key);
                    }
                    this._cd.detectChanges();
                    this.tableColumnchanges();
                });
        }
    }

    //#region Column Selector
    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.instrumentListColumns, listColumnMemoryCacheKey.instrumentColumn);
        let selectedColumn = instrumentListTableColumns.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.instrumentTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getInstrumentData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();

        this.instrumentTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });

        this.instrumentTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.instrumentTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }
    //#endregion

    private getInstrumentsDropdownInfo(): void {
        if (this.projectId) {
            this._commonService.getInstrumentDropdownInfo(this.projectId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.instrumentDropdownData = res;
                    this.autoCompleteValueChange();
                })
        }
    }

    private autoCompleteValueChange(): void {
        this.equipmentCodeFilteredOptions = this.setupFilteredOptions('equipmentCode', this.instrumentDropdownData?.equipmentCodeList || []);
        this.manufacturerFilteredOptions = this.setupFilteredOptions('manufacturer', this.instrumentDropdownData?.manufacturerList || []);
        this.natureOfSignalFilteredOptions = this.setupFilteredOptions('natureOfSignal', this.instrumentDropdownData?.natureOfSignalList || []);
        this.processFilteredOptions = this.setupFilteredOptions('process', this.instrumentDropdownData?.processList || []);
        this.plcNumberFilteredOptions = this.setupFilteredOptions('plcNo', this.instrumentDropdownData?.plcNumberList || []);
        this.tagFilteredOptions = this.setupFilteredOptions('tag', this.instrumentDropdownData?.tagList || []);
        this.zoneFilteredOptions = this.setupFilteredOptions('zone', this.instrumentDropdownData?.zoneList || []);
    }

    private setupFilteredOptions(field: string, dataList: DropdownInfoDtoModel[]): Observable<DropdownInfoDtoModel[]> {
        return this.field(field).valueChanges.pipe(
            startWith(''),
            map(val => val?.length >= 1 ? this._filter(val || '', dataList) : [])
        );
    }

    private _filter(value: string, dataList: DropdownInfoDtoModel[]): DropdownInfoDtoModel[] {
        const filterValue = value.toLowerCase();
        return dataList.filter(option => option?.name?.toLowerCase().includes(filterValue));
    }

    private getInstrumentData(): void {
        this.defaultCustomFilter();
        this._instrumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    protected onFileSelected(event: any): void {
        if (!event) return;

        const selectedFile = event.target.files[0] ?? null;
        if (!selectedFile) {
            this._toastr.error("Please select a file for import.");
            this.clearFileInput();
            return;
        }

        this._instrumentService.validateImportInstrument(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (!res || !res.isSucceeded) {
                        this._toastr.error("Validation failed. Please check your file.");
                        this.clearFileInput();
                        return;
                    }

                    const dialogRef = this.dialog.open(ImportPreviewDialogComponent, {
                        width: '750px',
                        data: res.records
                    });

                    dialogRef.afterClosed().subscribe((confirmed) => {
                        if (confirmed) {
                            this.proceedImport(selectedFile);
                            this.getInstrumentData();
                        } else {
                            this.clearFileInput();
                        }
                    });

                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "File validation failed.")
                }
        });
    }

    private proceedImport(selectedFile: File): void {

        this._instrumentService.importInstruments(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getInstrumentData();

                        if (res.records && res.records?.length > 0)
                            this._excelHelper.downloadImportResponseFile<[]>("Instruments", res.records, res.headers, true);
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();
                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "Import failed.");
                }
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const columnMapping = this.instrumentListColumns.filter(x => this.selectedColumns.includes(x.key)).map(c => c.label);
        const csvConfig = mkConfig({ filename: 'Sample_InstrumentList', columnHeaders: columnMapping, fieldSeparator: "," });
        const csv = generateCsv(csvConfig)([]);
        download(csvConfig)(csv);
    }

    private clearFileInput(): void {
        if (this.importFileInput)
            this.importFileInput.nativeElement.value = '';

        this._cd.detectChanges();
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}