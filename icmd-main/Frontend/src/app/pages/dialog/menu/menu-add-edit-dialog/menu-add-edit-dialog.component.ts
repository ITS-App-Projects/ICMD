import { CommonModule } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { CreateEditMenuFormComponent, CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { MenuService } from "src/app/service/menu";
import { MenuDialogInputDataModel, MenuDialogOutputDataModel } from "./menu-add-edit-dialog.model";
import { ProgressBarService } from "src/app/service/common";
import { ToastrService } from "ngx-toastr";
import { HttpErrorResponse } from "@angular/common/http";
import { BaseDataResponseModel } from "@m/auth/login-response-model";

@Component({
    standalone: true,
    selector: "app-menu-add-edit-dialog",
    templateUrl: "./menu-add-edit-dialog.component.html",
    providers: [
        MenuService,
    ],
    imports: [CommonModule, MatButtonModule, CreateEditMenuFormComponent, MatDialogModule],
})
export class MenuAddEditDialogComponent implements AfterViewInit, OnInit {
    @ViewChild("menuForm") menuForm: CreateEditMenuFormComponent;
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";

    constructor(
        private _dialogRef: MatDialogRef<
            MenuAddEditDialogComponent,
            MenuDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: MenuDialogInputDataModel,
        private _toastr: ToastrService,
        private _menuService: MenuService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService
    ) { }

    ngOnInit(): void {
        if (this._inputData.menuId != this.emptyGuid && this._inputData.menuId) {
            this._menuService.getMenuById(this._inputData.menuId).subscribe(
                (res) => {
                    if (res != null) {
                        this.menuForm.value = {
                            id: res?.id,
                            menuName: res?.menuName,
                            menuDescription: res?.menuDescription,
                            controllerName: res?.controllerName,
                            icon: res?.icon,
                            url: res?.url,
                            sortOrder: res?.sortOrder,
                            parentMenuId: res?.parentMenuId,
                            isPermission: res?.isPermission,
                        };

                        this.menuForm.field("menuName").clearValidators();
                        this.menuForm.field("menuName").updateValueAndValidity();
                        this._cdr.detectChanges();
                    }
                },
                (errorRes: HttpErrorResponse) => {
                    this._toastr.error(errorRes?.error?.message);
                }
            );
        }
    }
    ngAfterViewInit(): void {
        if (this._inputData.menuId == this.emptyGuid) {
            this.menuForm.field("sortOrder").setValue(this._inputData.sortOrder);
        }
        if (this._inputData.isSubMenu)
            this.menuForm.field("parentMenuId").setValue(this._inputData.mainMenuId);
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false, menu: null });
    }

    protected saveMenuInfo() {
        if (this.menuForm.value === null) {
            return;
        }

        const menuInfo: CreateOrEditMenuModel = {
            ...this.menuForm.value,
            parentMenuId:
                this.menuForm.value?.parentMenuId?.toString() == ""
                    ? null
                    : this.menuForm.value.parentMenuId,
        };
        this._menuService.createOrEditMenu(menuInfo).subscribe(
            (res: BaseDataResponseModel<CreateOrEditMenuModel>) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._dialogRef.close({ success: true, menu: res.data });
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