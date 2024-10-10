import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { FailStateInfoDtoModel, ListFailStateTableComponent } from "@c/masters/failState/list-failState-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importFailState } from "@u/constants";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { DialogsService } from "src/app/service/common";
import { FailStateDialogsService, FailStateSearchHelperService, FailStateService } from "src/app/service/failState";

@Component({
    standalone: true,
    selector: "app-list-failState-page",
    templateUrl: "./list-failState-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListFailStateTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        FailStateService,
        FailStateSearchHelperService,
        FailStateDialogsService,
        DialogsService,
        ExcelHelper
    ]
})
export class ListFailStatePageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListFailStateTableComponent) failStateTable: ListFailStateTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _failStateSearchHelperService: FailStateSearchHelperService,
        private _failStateService: FailStateService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        protected appConfig: AppConfig,
        private _failStateDialogService: FailStateDialogsService,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getFailStateData();
    }

    ngAfterViewInit(): void {
        this.failStateTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._failStateSearchHelperService.updateSortingChange(res);
        });

        this.failStateTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._failStateSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._failStateSearchHelperService.updateFilterChange(filter);
        });

        this.tableColumnchanges();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._failStateSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this fail State?",
            "Confirm"
        );
        if (isOk) {
            this._failStateService.deleteFailState($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getFailStateData();
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

    protected async addEditFailStateDialog(event: string = null): Promise<void> {
        await this._failStateDialogService.openFailStateDialog(event);
        this.getFailStateData();
    }

    protected exportData(): void {
        const fileName = 'Export_Fail_State';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._failStateSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'failStateName': 'Fail State Name'
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.failStateTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }
    

    private getFailStateData(): void {
        this.defaultCustomFilter();
        this._failStateSearchHelperService
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

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_FailState', columnHeaders: importFailState, fieldSeparator: "," });
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

        this._failStateService.importFailState
        (selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getFailStateData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<FailStateInfoDtoModel>("FailState", res.records, importFailState);

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