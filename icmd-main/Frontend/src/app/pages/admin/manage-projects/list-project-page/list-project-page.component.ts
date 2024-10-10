import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ListProjectTableComponent } from "@c/manage-projects/list-project-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ActiveInActiveDtoModel, CustomFieldSearchModel } from "@m/common";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Observable, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { CommonService, DialogsService } from "src/app/service/common";
import { ProjectDialogsService, ProjectSearchHelperService, ProjectService } from "src/app/service/manage-projects";
import { SearchProjectFilterModel } from "./list-project-page.model";
import { getGroup } from "@u/forms";
import { RecordType, SearchType } from "@e/common";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { ReactiveFormsModule } from "@angular/forms";
import { MatExpansionModule } from "@angular/material/expansion";
import { AppConfig } from "src/app/app.config";
import { ExcelHelper } from "@u/helper";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { listColumnMemoryCacheKey } from "@u/default";
import { manageProjectListTableColumns } from "@u/constants";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";

@Component({
    standalone: true,
    selector: "app-list-project-page",
    templateUrl: "./list-project-page.component.html",
    imports: [
        CommonModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        ListProjectTableComponent,
        MatDialogModule,
        MatSelectModule,
        MatExpansionModule,
        PermissionWrapperComponent
    ],
    providers: [DialogsService, ProjectService, ProjectSearchHelperService, ProjectDialogsService, ExcelHelper, CommonService, ColumnSelectorDialogsService]
})
export class ListProjectPageComponent extends FormBaseComponent<SearchProjectFilterModel> {
    @ViewChild(ListProjectTableComponent) projectTable: ListProjectTableComponent;
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    protected manageProjectListColumns = [...manageProjectListTableColumns.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(protected _projectSearchHelperService: ProjectSearchHelperService,
        private _dialog: DialogsService, private _projectService: ProjectService, private _toastr: ToastrService,
        private _projectDialogService: ProjectDialogsService,
        protected appConfig: AppConfig, private _excelHelper: ExcelHelper,
        private _commonService: CommonService, private _cd: ChangeDetectorRef, private _columnSelectorDialogService: ColumnSelectorDialogsService) {
        super(
            getGroup<SearchProjectFilterModel>({
                type: { v: RecordType.Active }
            })
        );
        const keys = Object.keys(this.recordTypeEnum);
        this.recordType = keys.slice(keys.length / 2);
        this.getProjectData();
    }


    ngAfterViewInit(): void {
        this.projectTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._projectSearchHelperService.updateSortingChange(res);
        });

        this.projectTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._projectSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._projectSearchHelperService.updateFilterChange(filter);
        });
        this.getMemoryCacheItem();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._projectSearchHelperService.commonSearch($event);
    }

    protected async deleteProject($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this project?",
            "Confirm"
        );
        if (isOk) {
            this._projectService.deleteProject($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.appConfig.isProjectUpdate$.next(true);
                        this.getProjectData();
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

    protected resetFilter() {
        this.form.reset();
        this.field('type').setValue(RecordType.Active);
        this.defaultCustomFilter();
    }

    protected async addEditProjectDialog(event: string = null): Promise<void> {
        await this._projectDialogService.openProjectDialog(event);
        this.getProjectData();
        this.appConfig.isProjectUpdate$.next(true);
    }

    protected async activeInactiveProject($event: ActiveInActiveDtoModel): Promise<void> {
        const msg = !$event.isActive
            ? 'Are you sure you want to activate this project?'
            : 'Are you sure you want to inactivate this project?';
        const isOk = await this._dialog.confirm(
            msg,
            'Confirm'
        );
        if (isOk) {
            $event.isActive = !$event.isActive;
            this._projectService.activeInActiveProject($event).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.appConfig.isProjectUpdate$.next(true);
                        this.getProjectData();
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

    protected exportData(): void {
        const fileName = 'Export_Projects';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._projectSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.manageProjectListColumns.filter(x => this.selectedColumns.includes(x.key)).reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;
        if (formValue.type != null) {
            filters.push({
                fieldName: "type",
                fieldValue: formValue.type?.toString(),
                searchType: SearchType.Contains, isColumnFilter: false
            });
        }
        filters.push(
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        )

        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)
        this.customFilters$.next(filters);
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.manageProjectListColumns, listColumnMemoryCacheKey.manageProject);
        let selectedColumn = manageProjectListTableColumns.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.projectTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getProjectData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.projectTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getProjectData(): void {
        this.defaultCustomFilter();
        this._projectSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.manageProject).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.projectTable.displayedColumns = [...this.selectedColumns, manageProjectListTableColumns[manageProjectListTableColumns.length - 1].key];
                } else {
                    this.selectedColumns = this.manageProjectListColumns.map(x => x.key);
                }
                this._cd.detectChanges();
                this.tableColumnchanges();
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}