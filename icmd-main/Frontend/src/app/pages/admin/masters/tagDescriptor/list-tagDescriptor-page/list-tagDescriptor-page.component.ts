import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { ListTagTypeTableComponent, TagTypeInfoDtoModel } from "@c/masters/tagType/list-tagType-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { ListActionsComponent } from "@c/shared/list-actions";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { SearchType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";
import { importTagDescriptorColumns, masterTagDescriptorListTableColumn } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ExcelHelper } from "@u/helper";
import { download, generateCsv, mkConfig } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";
import { TagDescriptorDialogsService, TagDescriptorSearchHelperService, TagDescriptorService } from "src/app/service/tagDescriptor";

@Component({
    standalone: true,
    selector: "app-list-tagDescriptor-page",
    templateUrl: "./list-tagDescriptor-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListTagTypeTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        TagDescriptorService,
        TagDescriptorSearchHelperService,
        DialogsService,
        ExcelHelper, CommonService,
        ColumnSelectorDialogsService,
        TagDescriptorDialogsService
    ]
})
export class ListTagDescriptorPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListTagTypeTableComponent) tagDescriptorTable: ListTagTypeTableComponent;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected tagDescriptorListColumns = [...masterTagDescriptorListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _tagDescriptorSearchHelperService: TagDescriptorSearchHelperService,
        private _tagDescriptorService: TagDescriptorService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _excelHelper: ExcelHelper,
        protected appConfig: AppConfig,
        private _tagDescriptionDialogService: TagDescriptorDialogsService,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getTagDescriptorsData();
    }

    ngAfterViewInit(): void {
        this.tagDescriptorTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._tagDescriptorSearchHelperService.updateSortingChange(res);
        });

        this.tagDescriptorTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._tagDescriptorSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._tagDescriptorSearchHelperService.updateFilterChange(filter);
        });

        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._tagDescriptorSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this tag descriptor?",
            "Confirm"
        );
        if (isOk) {
            this._tagDescriptorService.deleteTagDescriptor($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTagDescriptorsData();
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

    protected async addEditTagDescriptorDialog(event: string = null): Promise<void> {
        await this._tagDescriptionDialogService.openTagDescriptorDialog(event);
        this.getTagDescriptorsData();
    }

    protected exportData(): void {
        const fileName = 'Export_Tag_Descriptors';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._tagDescriptorSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = this.tagDescriptorListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }
    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.tagDescriptorListColumns, listColumnMemoryCacheKey.tagDescriptors);
        let selectedColumn = masterTagDescriptorListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.tagDescriptorTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getTagDescriptorsData();
        }
    }

    private getTagDescriptorsData(): void {
        this.defaultCustomFilter();
        this._tagDescriptorSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.tagDescriptorTable.columnFiltersList.map(x => x.columnFilterModel$))
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

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.tagDescriptors).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.tagDescriptorTable.displayedColumns = [...this.selectedColumns, masterTagDescriptorListTableColumn[masterTagDescriptorListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.tagDescriptorListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_TagDescriptor', columnHeaders: importTagDescriptorColumns, fieldSeparator: "," });
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

        this._tagDescriptorService.importTagDescriptor(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTagDescriptorsData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<TagTypeInfoDtoModel>("TagDescriptor", res.records, importTagDescriptorColumns);

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