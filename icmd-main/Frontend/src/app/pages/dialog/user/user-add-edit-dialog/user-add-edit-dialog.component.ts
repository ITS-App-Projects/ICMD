import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { UserService } from "src/app/service/manage-user";
import { UserDialogInputDataModel, UserDialogOutputDataModel } from ".";
import { ProgressBarService } from "src/app/service/common";
import { CreateOrEditUserFormComponent, CreateOrEditUserModel, UpdateUserModel, ViewUserDetails } from "@c/manange-user/create-edit-user-form";
import { MatIconModule } from "@angular/material/icon";

@Component({
    standalone: true,
    selector: "app-user-dialog",
    templateUrl: "./user-add-edit-dialog.component.html",
    providers: [
        UserService,
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditUserFormComponent,
        MatDialogModule
    ],
})
export class UserAddEditDialogComponent implements AfterViewInit, OnInit {
    @ViewChild("userForm") userForm: CreateOrEditUserFormComponent;
    emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    private userInfo: ViewUserDetails;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            UserAddEditDialogComponent,
            UserDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: UserDialogInputDataModel,
        private _toastr: ToastrService,
        private _userService: UserService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService
    ) { }

    ngOnInit(): void { }

    ngAfterViewInit(): void {
        this.userForm.roleInfo = this._inputData.roles;
        if (this._inputData.id && this._inputData.id != "" && this._inputData.id != this.emptyGuid) {
            this._userService.getUserInfo(this._inputData.id).subscribe(
                (res) => {
                    if (res != null) {
                        this.userInfo = res;
                        this.userForm.value = {
                            id: res?.id,
                            firstName: res?.firstName,
                            lastName: res?.lastName,
                            userName: res?.userName,
                            email: res?.email,
                            phoneNumber: res?.phoneNumber,
                            roleName: res?.roleName,
                            password: "",
                            confirmPassword: "",
                        };

                        this.userForm.field("password").clearValidators();
                        this.userForm.field("password").updateValueAndValidity();
                        this.userForm.field("confirmPassword").clearValidators();
                        this.userForm.field("confirmPassword").updateValueAndValidity();
                        this._cdr.detectChanges();
                    }
                },
                (errorRes: HttpErrorResponse) => {
                    this._toastr.error(errorRes?.error?.message);

                }
            );
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveUserInfo() {
        if (this.userForm.value === null) {
            return;
        }
        this.isLoading = !this.isLoading;
        if (this.userForm.value.id == this.emptyGuid) {
            const userInfo: CreateOrEditUserModel = {
                ...this.userForm.value
            };
            this._userService.createUser(userInfo).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this._dialogRef.close({ success: true });
                    } else {
                        this.isLoading = !this.isLoading;
                        this._toastr.error(res.message);
                    }
                },
                (errorRes: HttpErrorResponse) => {
                    if (errorRes?.error?.message) {
                        this._toastr.error(errorRes?.error?.message);
                    }
                    this.isLoading = !this.isLoading;
                }
            );
        } else {
            const userInfo: UpdateUserModel = {
                ...this.userForm.value,
            };
            this._userService.updateUser(userInfo).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this._dialogRef.close({ success: true });
                    } else {
                        this._toastr.error(res.message);
                    }
                },
                (errorRes: HttpErrorResponse) => {
                    if (errorRes?.error?.message) {
                        this._toastr.error(errorRes?.error?.message);
                    }
                }
            );
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}