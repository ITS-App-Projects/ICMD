import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { ListUserTableComponent } from "@c/manange-user/list-user-table";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { RoleDetailsModel } from "@m/common/role-details.model";
import { ToastrService } from "ngx-toastr";
import { BehaviorSubject, Subject, combineLatest } from "rxjs";
import { take, takeUntil } from "rxjs/operators";
import { CommonService, DialogsService } from "src/app/service/common";
import { UserChangePasswordDialogService, UserSearchHelperService, UserService } from "src/app/service/manage-user";
import { UserDialogsService } from "src/app/service/manage-user/user-dialogs-service";
import { SearchUserFilterModel } from ".";
import { getGroup } from "@u/forms";
import { CustomFieldSearchModel } from "@m/common";
import { RecordType, SearchType } from "@e/common";
import { ExcelHelper } from "@u/helper";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { manangeUserListTableColumns } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ColumnSelectorDialogsService } from "src/app/service/column-selector";

@Component({
    standalone: true,
    selector: "app-list-user-page",
    templateUrl: "./list-user-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatFormFieldModule,
        ListUserTableComponent,
        MatDialogModule,
        MatInputModule,
        ReactiveFormsModule,
        MatSelectModule,
        MatExpansionModule,
        PermissionWrapperComponent
    ],
    providers: [
        UserService,
        DialogsService,
        UserDialogsService,
        UserSearchHelperService,
        ExcelHelper,
        CommonService,
        ColumnSelectorDialogsService,
        UserChangePasswordDialogService
    ]
})
export class ListUserPageComponent extends FormBaseComponent<SearchUserFilterModel> {
    @ViewChild(ListUserTableComponent) userTable: ListUserTableComponent;
    protected roles: RoleDetailsModel[];
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();

    protected manageUsersListColumns = [...manangeUserListTableColumns.filter(x => x.key != 'actions')];
    private selectedColumns: string[] = [];
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _userSearchHelperService: UserSearchHelperService,
        private _userDialogService: UserDialogsService,
        private _userService: UserService,
        private _toastr: ToastrService,
        private _dialog: DialogsService, private _excelHelper: ExcelHelper,
        protected appConfig: AppConfig,
        private _commonService: CommonService,
        private _cd: ChangeDetectorRef,
        private _columnSelectorDialogService: ColumnSelectorDialogsService,
        private _userChangePasswordDialogService: UserChangePasswordDialogService) {
        super(
            getGroup<SearchUserFilterModel>({
                role: { v: [] },
                type: { v: RecordType.Active }
            })
        );
        const keys = Object.keys(this.recordTypeEnum);
        this.recordType = keys.slice(keys.length / 2);
        this.getUserData();
        this.getallRoles();
    }

    ngOnInit() { }

    ngAfterViewInit(): void {
        this.userTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._userSearchHelperService.updateSortingChange(res);
        });

        this.userTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._userSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._userSearchHelperService.updateFilterChange(filter);
        });
        this.getMemoryCacheItem();
    }

    private getUserData(): void {
        this.defaultCustomFilter();
        this._userSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    async addEditUserDialog(event: string = null): Promise<void> {
        await this._userDialogService.openUserDialog(event, this.roles);
        this.getUserData();
    }

    protected async passwordChangeDialog(event: string = null) {
        await this._userChangePasswordDialogService.openChangePasswordDialog(event);
        this.getUserData();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._userSearchHelperService.commonSearch($event);
    }

    async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this user?",
            "Confirm"
        );
        if (isOk) {
            this._userService.deleteUser($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getUserData();
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

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [];
        const formValue = this.form.value;
        if (formValue.role != null && formValue.role.length != 0) {
            filters.push({
                fieldName: "roles",
                fieldValue: formValue.role?.join(","),
                searchType: SearchType.Contains, isColumnFilter: false
            });
        }

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

    protected resetFilter() {
        this.form.reset();
        this.field('type').setValue(RecordType.Active);
        this.defaultCustomFilter();
    }

    protected exportData(): void {
        const fileName = 'Export_Users';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._userSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = this.manageUsersListColumns.filter(x => this.selectedColumns.includes(x.key))
                .reduce((acc, column) => {
                    acc[column.key] = column.label;
                    return acc;
                }, {});
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    protected async openColumnSelectorDialog() {
        const data = await this._columnSelectorDialogService.openColumnSelectorDialog(this.manageUsersListColumns, listColumnMemoryCacheKey.manageUser);
        let selectedColumn = manangeUserListTableColumns.map(x => x.key);
        if (data.selectedColumns.length > 0)
            selectedColumn = data.selectedColumns;

        if (data.success) {
            this.selectedColumns = selectedColumn;
            this.userTable.displayedColumns = this.selectedColumns;
            this._cd.detectChanges();
            this.tableColumnchanges();
            this.getUserData();
        }
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.userTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

    private getallRoles(): void {
        this._userService
            .getallRoles()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res: RoleDetailsModel[]) => {
                this.roles = res;
            });
    }

    private getMemoryCacheItem(): void {
        this._commonService.getMemoryCacheItem(listColumnMemoryCacheKey.manageUser).pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const selectedColumn = res;
                if (selectedColumn != null && selectedColumn.length > 0) {
                    this.selectedColumns = selectedColumn;
                    this.userTable.displayedColumns = [...this.selectedColumns, manangeUserListTableColumns[manangeUserListTableColumns.length - 1].key];
                } else {
                    this.selectedColumns = this.manageUsersListColumns.map(x => x.key);
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