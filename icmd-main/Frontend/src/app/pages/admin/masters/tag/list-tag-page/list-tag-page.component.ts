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
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import { ListTagTableComponent } from '@c/masters/tag/list-tag-table';
import { TagBulkDialogComponent } from '@c/shared/bulkDelete-dialog/project-master/tags-master/tag-bulk-dialog.component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import {
  CustomFieldSearchModel,
  ProjectTagFieldInfoDtoModel
} from '@m/common';
import { importTagDescriptorColumns, masterTagListTableColumn } from '@u/constants';
import { listColumnMemoryCacheKey } from '@u/default';
import { ExcelHelper } from '@u/helper';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';
import { TagInfoDtoModel } from '@c/masters/tag/create-edit-tag-form';

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
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _tagSearchHelperService: TagSearchHelperService,
        private _tagService: TagService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
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
        });

        this.tagTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });
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

    //#region Delete Bulk
    protected async deleteBulkTag(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(TagBulkDialogComponent, {
            width: "600",
            data: ids
        });

        dialogRef.afterClosed().subscribe((result: string[] | null) => {
            if (result) {
                this._tagService.deleteBulkTag(result).pipe(takeUntil(this._destroy$)).subscribe(
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
        });
    }

    protected async addEditTagDialog(event: string = null): Promise<void> {
        await this._tagDialogService.openTagDialog(event, this.projectId);
        this.getTagData();
    }

    protected async bulkEditTag(): Promise<void> {
        const fileName = "Edit_Tags"

        this.defaultCustomFilter(true, this.columnFilterList);
        this._tagSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping: Record<string, string> = [{ key: 'id', label: 'Id' }]
                .concat(masterTagListTableColumn.filter(({ key }) => key !== 'actions'))
                .reduce((acc, { key, label }) => {
                    acc[`${key}`] = `${label}`;
                    return acc;
                }, {} as Record<string, string>);

                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
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
        this.tagTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.tagTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.tagTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
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

        this._tagService.validateImportTag(this.projectId, selectedFile)
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
                            this.getTagData();
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

        this._tagService.importTag(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getTagData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<[]>("Tag", res.records, res.headers, true );
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