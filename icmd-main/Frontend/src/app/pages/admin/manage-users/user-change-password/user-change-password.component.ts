import { HttpErrorResponse } from "@angular/common/http";
import { Component, Input } from "@angular/core";
import { Validators } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { FormDefaultsModule, FormBaseComponent } from "@c/shared/forms";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { getGroup } from "@u/forms";
import { PasswordStrengthValidator, mustMatch } from "@u/validators";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ProgressBarService } from "src/app/service/common";
import { ChangePasswordModel } from "./user-change-password.model";
import { UserService } from "src/app/service/manage-user";
import { AuthService } from "@module/auth";
import { MatButtonModule } from "@angular/material/button";

@Component({
    standalone: true,
    selector: "app-user-change-password",
    templateUrl: "./user-change-password.component.html",
    imports: [
        FormDefaultsModule,
        MatFormFieldModule,
        MatInputModule,
        MatIconModule,
        MatButtonModule
    ],
    providers: [
        UserService,
        AuthService
    ],
})
export class UserChangePasswordComponent extends FormBaseComponent<ChangePasswordModel> {
    @Input() userId: string = "";
    private _destroy$ = new Subject<void>();

    constructor(
        protected progressBarService: ProgressBarService,
        private _appConfig: AppConfig,
        private _userService: UserService,
        private auth: AuthService,
        private _toastr: ToastrService,
    ) {
        super(
            getGroup<ChangePasswordModel>(
                {
                    userId: { vldtr: [Validators.required] },
                    oldPassword: {
                        vldtr: [
                            Validators.required,
                            Validators.minLength(8),
                            PasswordStrengthValidator,
                        ],
                    },
                    newPassword: {
                        vldtr: [
                            Validators.required,
                            Validators.minLength(8),
                            PasswordStrengthValidator,
                        ],
                    },
                    confirmPassword: { vldtr: [Validators.required] },
                },
                mustMatch("newPassword", "confirmPassword", true)
            )
        );
    }

    ngOnInit() { }

    ngAfterViewInit() {
        this.field("userId").setValue(this.userId);
    }

    protected updatePassword() {
        if (this.value === null) {
            return;
        }

        this._userService
            .updateCurrentUserPassword(this.value)
            .pipe(takeUntil(this._destroy$))
            .subscribe(
                (res: BaseResponseModel) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.auth.logout().pipe(takeUntil(this._destroy$)).subscribe();
                    } else {
                        this._toastr.error(res.message);
                    }
                },
                (errorRes: HttpErrorResponse) => {
                    this._toastr.error(errorRes?.error?.message);
                }
            );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}