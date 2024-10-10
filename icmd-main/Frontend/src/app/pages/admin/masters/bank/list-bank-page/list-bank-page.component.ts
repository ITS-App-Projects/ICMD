import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { BankInfoDtoModel, ListBankTableComponent } from "@c/masters/bank/list-bank-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { ExcelHelper } from "@u/helper";
import { importBankFileColumns } from "@u/index";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { BankDialogsService, BankSearchHelperService, BankService } from "src/app/service/bank";
import { DialogsService } from "src/app/service/common";
import { ProjectService } from "src/app/service/manage-projects";

@Component({
    standalone: true,
    selector: "app-list-bank-page",
    templateUrl: "./list-bank-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListBankTableComponent,
        MatDialogModule,
        MatExpansionModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        BankService,
        BankSearchHelperService,
        BankDialogsService,
        DialogsService,
        ProjectService,
        ExcelHelper
    ]
})
export class ListBankPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListBankTableComponent) bankTable: ListBankTableComponent;
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _bankSearchHelperService: BankSearchHelperService,
        private _bankService: BankService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _projectService: ProjectService, private _bankDialogService: BankDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getBankData();
    }

    ngAfterViewInit(): void {
        this.bankTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._bankSearchHelperService.updateSortingChange(res);
        });

        this.bankTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._bankSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._bankSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this._cd.detectChanges();
                this.tableColumnchanges();
                this.getBankData();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._bankSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this bank?",
            "Confirm"
        );
        if (isOk) {
            this._bankService.deleteBank($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getBankData();
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

    protected async addEditBankDialog(event: string = null): Promise<void> {
        await this._bankDialogService.openBankDialog(event, this.projectId);
        this.getBankData();
    }

    protected exportData(): void {
        const fileName = 'Export_Bank';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._bankSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'bank': 'Bank',
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.bankTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getBankData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._bankSearchHelperService
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

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Bank', columnHeaders: importBankFileColumns, fieldSeparator: "," });
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


        this._bankService.importBank(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getBankData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<BankInfoDtoModel>("Bank", res.records, importBankFileColumns);

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