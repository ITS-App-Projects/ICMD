import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelect, MatSelectModule } from "@angular/material/select";
import { ListPermissionManagementTableComponent } from "@c/permission/list-permission-management-table";
import { RoleEnum } from "@e/common";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { RoleDetailsModel } from "@m/common";
import { MenuPermissionListModel, PermissionByMenuRoleModel } from "@m/permission";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { UserService } from "src/app/service/manage-user";
import { PermissionService } from "src/app/service/permission";


@Component({
    standalone: true,
    selector: "app-permission-page",
    templateUrl: "./permission-page.component.html",
    imports: [
        CommonModule,
        ListPermissionManagementTableComponent,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule
    ],
    providers: [PermissionService, UserService],
})
export class PermissionPageComponent implements OnInit {
    @ViewChild(ListPermissionManagementTableComponent)
    permissionTable: ListPermissionManagementTableComponent;
    @ViewChild("menus") matSelect: MatSelect;
    protected rolesList: RoleDetailsModel[] = [];
    protected mainMenuList: string[] = [];
    private allPermissionList: MenuPermissionListModel[] = [];
    protected isLoading: boolean = false;

    protected defaultSelected: string;
    private permissionByMenuRoleModel: PermissionByMenuRoleModel[] = [];
    private _destroy$ = new Subject<void>();
    constructor(
        private _userService: UserService,
        private _toastr: ToastrService,
        private _permissionService: PermissionService,
        private _cd: ChangeDetectorRef
    ) {
        this._userService
            .getallRoles()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.rolesList = res;

                this.defaultSelected = this.rolesList.find(
                    (x) => x.name == RoleEnum.Administrator
                ).id;
                this.getAllPermissionInfo(this.defaultSelected);
                this._cd.detectChanges();
            });
    }

    ngOnInit(): void { }

    ngAfterViewInit() {
        this.permissionTable.isPermissionChanged
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const isExist = this.permissionByMenuRoleModel.findIndex(
                    (x) =>
                        x.menuPermissionId == res.menuPermissionId &&
                        x.operation == res.operation
                );
                if (isExist > -1) this.permissionByMenuRoleModel.splice(isExist, 1);

                this.permissionByMenuRoleModel.push({
                    isGranted: res.isGranted,
                    menuPermissionId: res.menuPermissionId,
                    operation: res.operation,
                });
            });
    }

    protected getAllPermissionInfo(roleId: string) {
        this.mainMenuList = [];
        this.isLoading = true;
        this._permissionService
            .GetAllMenuWithPermission(roleId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((model: MenuPermissionListModel[]) => {
                this.permissionByMenuRoleModel = [];
                this.allPermissionList = model;
                this.permissionTable.menuListResponse = model;
                this.permissionTable.isSystemAdmin = this.rolesList.some(
                    (x) => x.name == RoleEnum.Administrator && x.id == roleId
                );

                this.mainMenuList =
                    model?.map((x: MenuPermissionListModel) => x.menuName) ?? [];

                this.matSelect.value = "";
                this.isLoading = false;
                this._cd.detectChanges();
            },
                (errorRes: HttpErrorResponse) => {
                    this._toastr.error(errorRes?.error?.message);
                    this.isLoading = false;
                });
    }

    protected savePermissions() {
        if (this.permissionByMenuRoleModel.length > 0) {
            this.isLoading = true;
            this._permissionService
                .setPermissionByRole(this.permissionByMenuRoleModel)
                .pipe(takeUntil(this._destroy$))
                .subscribe(
                    (res: BaseResponseModel) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                        } else {
                            this._toastr.error(res.message);
                        }
                        this.isLoading = false;
                    },
                    (errorRes: HttpErrorResponse) => {
                        this._toastr.error(errorRes?.error?.message);
                        this.isLoading = false;
                    }
                );
        }
    }

    protected getSelectedMenu(menuName: string) {
        if (menuName) {
            this.permissionTable.menuListResponse = this.allPermissionList.filter(
                (x: MenuPermissionListModel) => x.menuName == menuName
            );
        } else {
            this.permissionTable.menuListResponse = this.allPermissionList;
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
