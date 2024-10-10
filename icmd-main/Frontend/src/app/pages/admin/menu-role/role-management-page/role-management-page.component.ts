import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { Component, OnInit, ViewChild } from "@angular/core";
import { ListRoleTableComponent } from "@c/menu-role/list-role-table";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { MenuListWithRoleModel, RoleMenuPermissionModel } from "@m/menu";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { MenuService } from "src/app/service/menu";

@Component({
    standalone: true,
    selector: "app-role-management-page",
    templateUrl: "./role-management-page.component.html",
    imports: [CommonModule, ListRoleTableComponent],
    providers: [
        MenuService,
    ],
})
export class RoleManagementPageComponent {
    @ViewChild(ListRoleTableComponent) roleTable: ListRoleTableComponent;

    protected rolePermissionModel: RoleMenuPermissionModel[] = [];
    private _destroy$ = new Subject<void>();
    constructor(
        private _menuservice: MenuService,
        private _toastr: ToastrService
    ) {
        this.getAllMenuWithPermissionData();
    }

    ngAfterViewInit() {
        this.roleTable.isPermissionChanged
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                const isExist = this.rolePermissionModel.findIndex(
                    (x) => x.menuId == res.menuId && x.roleId == res.roleId
                );
                if (isExist > -1) this.rolePermissionModel.splice(isExist, 1);

                this.rolePermissionModel.push({
                    isGranted: res.isGranted,
                    menuId: res.menuId,
                    roleId: res.roleId,
                });
            });
    }

    protected saveRolePermission() {
        if (this.rolePermissionModel.length > 0) {
            this._menuservice
                .saveRolePermission(this.rolePermissionModel)
                .pipe(takeUntil(this._destroy$))
                .subscribe(
                    (res: BaseResponseModel) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes: HttpErrorResponse) => {
                        this._toastr.error(errorRes?.error?.message);
                    }
                );
        }
    }

    private getAllMenuWithPermissionData() {
        this._menuservice
            .getAllWithPermission()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model: MenuListWithRoleModel) => {
                this.roleTable.menuListResponse = model;
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}