import {
  download,
  generateCsv,
  mkConfig
} from 'export-to-csv';
import { ToastrService } from 'ngx-toastr';
import {
  combineLatest,
  BehaviorSubject,
  Subject
} from 'rxjs';
import {
  take,
  takeUntil
} from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { ColumnSelectorDialogsService } from 'src/app/service/column-selector';
import {
  CommonService,
  DialogsService
} from 'src/app/service/common';
import { ProjectService } from 'src/app/service/manage-projects';
import {
  TagDialogsService,
  TagSearchHelperService,
  TagService
} from 'src/app/service/tag';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { ListTagTableComponent } from '@c/masters/tag/list-tag-table';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import {
  CustomFieldSearchModel,
  ProjectTagFieldInfoDtoModel
} from '@m/common';
import { masterTagListTableColumn } from '@u/constants';
import { listColumnMemoryCacheKey } from '@u/default';
import { ExcelHelper } from '@u/helper';

@Component({
    standalone: true,
    selector: "app-list-tag-page",
    templateUrl: "./list-tag-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListTagTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        TagService,
        TagSearchHelperService,
        DialogsService,
        ProjectService,
        ExcelHelper,
        TagDialogsService, CommonService,
        ColumnSelectorDialogsService
    ]
})
export class ListTagPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListTagTableComponent) tagTable: ListTagTableComponent;
    protected projectTagFieldData: ProjectTagFieldInfoDtoModel[] = [];
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected tagListColumns = [...masterTagListTableColumn.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _tagSearchHelperService: TagSearchHelperService,
        private _tagService: TagService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private _projectService: ProjectService,
        protected appConfig: AppConfig,
        private _cdr: ChangeDetectorRef,
        private _tagDialogService: TagDialogsService,
        private _excelHelper: ExcelHelper,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _cd: ChangeDetectorRef,
        private _commonService: CommonService) {
        this.getTagData();
    }

    ngAfterViewInit(): void {
        this.tagTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._tagSearchHelperService.updateSortingChange(res);
        });

        this.tagTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._tagSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._tagSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getAllProjectTagFieldData();
                this.getTagData();
                this.getMemoryCacheItem();
            }
        })
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._tagSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this tag?",
            "Confirm"
        );
        if (isOk) {
            this._tagService.deleteTag($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTagData();
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

    protected async addEditTagDialog(event: string = null): Promise<void> {
        await this._tagDialogService.openTagDialog(event, this.projectId);
        this.getTagData();
    }

    protected exportData(): void {
        const fileName = 'Export_Tags';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._tagSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.tagListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.tagListColumns, listColumnMemoryCacheKey.tags);
        let selectedColumn = masterTagListTableColumn.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.tagTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getTagData();
        }
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        filters.push(
            { fieldName: "projectIds", fieldValue: this.projectId, searchType: SearchType.Contains, isColumnFilter: false },
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        )
        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)
        this.customFilters$.next(filters);
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.tagTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getTagData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._tagSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    private getAllProjectTagFieldData(): void {
        if (this.projectId) {
            this._projectService.getProjectTagFieldSourcesDataInfo(this.projectId).subscribe((res) => {
                this.projectTagFieldData = res;
                this.tagTable.projectTagFieldData = res;

                this.projectTagFieldData?.forEach((element: ProjectTagFieldInfoDtoModel, index: number) => {
                    this.tagListColumns[index + 1].label = element?.name ?? "";
                });

                this._cdr.detectChanges();
            })
        }
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.tags).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.tagTable.displayedColumns = [...this.selectedColumns, masterTagListTableColumn[masterTagListTableColumn.length - 1].key];
                } else {
                    this.selectedColumns = this.tagListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    //#region Import Functionality
    protected importFileDownload() {
        this._projectService.getProjectTagFieldSourcesDataInfo(this.projectId).pipe(takeUntil(this._destroy$)).subscribe((result) => {
            let headers: string[] = [];
            const itemCounts: Map<string, number> = new Map();
            headers.push("Tag");
            result.forEach((item) => {
                if (item.isUsed) {
                    if (!itemCounts.has(item.name)) {
                        itemCounts.set(item.name, 1);
                        headers.push(item.name);
                    } else {
                        const count = itemCounts.get(item.name)! + 1;
                        itemCounts.set(item.name, count);
                        headers.push(`${item.name}${count}`);
                    }
                }
            });
            const csvConfig = mkConfig({ filename: 'Sample_Tag', columnHeaders: headers, fieldSeparator: "," });
            const csv = generateCsv(csvConfig)([]);
            download(csvConfig)(csv);
        });
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

        this._tagService.importTag(this.projectId, selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTagData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<[]>("Tag", res.records, res.headers, true);

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