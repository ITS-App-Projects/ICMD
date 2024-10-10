import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { UserChangePasswordDialogInputDataModel, UserChangePasswordDialogOutputDataModel } from ".";
import { CommonModule } from "@angular/common";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { UserService } from "src/app/service/manage-user";
import { ChangeUserPasswordFormComponent, ChangeUserPasswordModel } from "@c/manange-user/change-user-password-form";
import { takeUntil } from "rxjs/operators";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
    standalone: true,
    selector: "app-change-user-password-dialog",
    templateUrl: "./user-change-password-dialog.component.html",
    providers: [
        UserService,
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        ChangeUserPasswordFormComponent,
        MatDialogModule
    ],
})
export class UserChangePasswordDialogComponent implements AfterViewInit, OnInit {
    @ViewChild("userForm") userForm: ChangeUserPasswordFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            UserChangePasswordDialogComponent,
            UserChangePasswordDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: UserChangePasswordDialogInputDataModel,
        private _toastr: ToastrService,
        private _userService: UserService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService
    ) { }

    ngOnInit(): void { }

    ngAfterViewInit(): void {
        this.userForm.field("id").setValue(this._inputData.id);
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveInfo() {
        if (this.userForm.value === null) {
            return;
        }
        this.isLoading = !this.isLoading;
        if (this.userForm.value.id) {
            const userInfo: ChangeUserPasswordModel = {
                ...this.userForm.value
            };

            this._userService.updateUserPassword(userInfo).pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._dialogRef.close({ success: true });
                } else {
                    this.isLoading = !this.isLoading;
                    this._toastr.error(res.message);
                }
            }, (errorRes: HttpErrorResponse) => {
                if (errorRes?.error?.message) {
                    this._toastr.error(errorRes?.error?.message);
                }
                this.isLoading = !this.isLoading;
            }
            );
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}