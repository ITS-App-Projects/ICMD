import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { ListNonInstrumentTableComponent } from "@c/nonInstrument-list/list-nonInstrument-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ExcelHelper } from "@u/helper";
import { ProjectService } from "src/app/service/manage-projects";
import { NonInstrumentSearchHelperService, NonInstrumentService } from "src/app/service/non-instrument";
import { NonInstrumentDropdownInfoDtoModel, SearchNonInstrumentFilterModel } from "./list-nonInstrument-page.model";
import { ActiveInActiveDtoModel, CustomFieldSearchModel, DropdownInfoDtoModel } from "@m/common";
import { BehaviorSubject, Observable, Subject, combineLatest, forkJoin } from "rxjs";
import { RecordType, SearchType } from "@e/common";
import { ToastrService } from "ngx-toastr";
import { AppConfig } from "src/app/app.config";
import { getGroup } from "@u/forms";
import { map, startWith, take, takeUntil } from "rxjs/operators";
import { CommonService, DialogsService } from "src/app/service/common";
import { MatDialogModule } from "@angular/material/dialog";
import { AppRoute } from "@u/app.route";
import { Router } from "@angular/router";
import { DeviceService } from "src/app/service/device";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { nonInstrumentListTableColumns } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";

@Component({
    standalone: true,
    selector: "app-list-nonInstrument-page",
    templateUrl: "./list-nonInstrument-page.component.html",
    imports: [
        CommonModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule,
        MatExpansionModule,
        ListNonInstrumentTableComponent,
        MatAutocompleteModule,
        MatDialogModule,
        PermissionWrapperComponent
    ],
    providers: [DialogsService, NonInstrumentSearchHelperService, ProjectService, NonInstrumentService, ExcelHelper, DeviceService, CommonService, ColumnSelectorDialogsService]
})
export class ListNonInstrumentPageComponent extends FormBaseComponent<SearchNonInstrumentFilterModel> {
    @ViewChild(ListNonInstrumentTableComponent) nonInstrumentTable: ListNonInstrumentTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected searchTypesEnum = SearchType;
    protected searchTypes: string[] = [];
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    protected equipmentCodeFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected plcNumberFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected tagFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected locationFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    private projectId: string | null = null;
    private tagFieldNames: string[] = [];
    private nonInstrumentDropdownData: NonInstrumentDropdownInfoDtoModel;

    protected nonInstrumentListColumns = [...nonInstrumentListTableColumns.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(protected _nonInstrumentSearchHelperService: NonInstrumentSearchHelperService,
        private _projectService: ProjectService,
        private _nonInstrumentService: NonInstrumentService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _router: Router,
        private _commonService: CommonService,
        private _deviceService: DeviceService,
        protected appConfig: AppConfig, private _excelHelper: ExcelHelper, private _cd: ChangeDetectorRef,
        private _columnSelectorDialogService: ColumnSelectorDialogsService) {
        super(
            getGroup<SearchNonInstrumentFilterModel>({
                equipmentCode: {},
                equipmentCodeSearchType: { v: SearchType.Contains },
                plcNo: {},
                plcNoSearchType: { v: SearchType.Contains },
                tag: {},
                tagSearchType: { v: SearchType.Contains },
                location: {},
                locationSearchType: { v: SearchType.Contains },
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
                this.getNonInstrumentData();
                this._nonInstrumentSearchHelperService.updateProjectId(this.projectId);
                this.getNonInstrumentsDropdownInfo();
            }
        });

        this.nonInstrumentTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._nonInstrumentSearchHelperService.updateSortingChange(res);
        });

        this.nonInstrumentTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._nonInstrumentSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._nonInstrumentSearchHelperService.updateFilterChange(filter);
        });
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._nonInstrumentSearchHelperService.commonSearch($event);
    }

    protected resetFilter() {
        this.form.reset();
        this.field('tagSearchType').setValue(SearchType.Contains);
        this.field('plcNoSearchType').setValue(SearchType.Contains);
        this.field('equipmentCodeSearchType').setValue(SearchType.Contains);
        this.field('locationSearchType').setValue(SearchType.Contains);
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
                fieldName: "plcNumber",
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

        if (formValue.location != null && formValue.location != "") {
            filters.push({
                fieldName: "location",
                fieldValue: formValue.location?.toString(),
                searchType: formValue.locationSearchType,
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
        const fileName = 'Export_Non_Instruments';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._nonInstrumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.nonInstrumentListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });


    }

    protected getProjectTagFieldName(projectId: string): void {
        if (projectId) {
            forkJoin([
                this._projectService.getProjectTagFieldNames(projectId),
                this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.nonInstrumentColumn)
            ]).pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.tagFieldNames = [];
                    this.nonInstrumentTable.tagFieldNames = [];
                    let array: string[] = [];
                    res[0].map((item, index) => {
                        array.push(item);
                    })
                    this.tagFieldNames = array;
                    this.nonInstrumentTable.tagFieldNames = array;

                    this.nonInstrumentTable.tagFieldNames.forEach((element: string, index: number) => {
                        this.nonInstrumentListColumns[index].label = element;
                    });

                    const selectedColumn = res[1];
                    if (selectedColumn != null && selectedColumn.length > 0) {
                        this.selectedColumns = selectedColumn;
                        this.nonInstrumentTable.displayedColumns = [...this.selectedColumns, nonInstrumentListTableColumns[nonInstrumentListTableColumns.length - 1].key];
                    } else {
                        this.selectedColumns = this.nonInstrumentListColumns.map(x => x.key);
                    }
                    this._cd.detectChanges();
                    this.tableColumnchanges();
                });
        }
    }

    protected showDevice(event: string = null): void {
        this.appConfig.isPreviousURL$ = AppRoute.nonInstrumentList;
        this._router.navigate([AppRoute.manageDevice, event ?? ""]);
    }

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
                        this.getNonInstrumentData();
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
                        this.getNonInstrumentData();
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

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.nonInstrumentListColumns, listColumnMemoryCacheKey.nonInstrumentColumn);
        let selectedColumn = nonInstrumentListTableColumns.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.nonInstrumentTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getNonInstrumentData();
        }
        this._cd.detectChanges();
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.nonInstrumentTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getNonInstrumentData(): void {
        this.defaultCustomFilter();
        this._nonInstrumentSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    private getNonInstrumentsDropdownInfo(): void {
        if (this.projectId) {
            this._commonService.getNonInstrumentDropdownInfo(this.projectId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.nonInstrumentDropdownData = res;
                    this.autoCompleteValueChange();
                })
        }
    }

    private autoCompleteValueChange(): void {
        this.tagFilteredOptions = this.setupFilteredOptions('tag', this.nonInstrumentDropdownData?.tagList || []);
        this.plcNumberFilteredOptions = this.setupFilteredOptions('plcNo', this.nonInstrumentDropdownData?.plcNumberList || []);
        this.equipmentCodeFilteredOptions = this.setupFilteredOptions('equipmentCode', this.nonInstrumentDropdownData?.equipmentCodeList || []);
        this.locationFilteredOptions = this.setupFilteredOptions('location', this.nonInstrumentDropdownData?.locationList || []);
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

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}