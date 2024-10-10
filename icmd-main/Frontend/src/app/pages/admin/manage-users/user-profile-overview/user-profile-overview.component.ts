import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { Component, EventEmitter, Output } from "@angular/core";
import { FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { UpdateUserModel, ViewUserDetails } from "@c/manange-user/create-edit-user-form";
import { FormBaseComponent } from "@c/shared/forms";
import { NgbTooltipModule } from "@ng-bootstrap/ng-bootstrap";
import { getGroup } from "@u/forms";
import { ToastrService } from "ngx-toastr";
import { AppConfig } from "src/app/app.config";
import { ProgressBarService } from "src/app/service/common";
import { UserService } from "src/app/service/manage-user";

@Component({
    standalone: true,
    selector: "app-user-profile-overview",
    templateUrl: "./user-profile-overview.component.html",
    providers: [
        UserService
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        NgbTooltipModule,
        MatInputModule,
        MatSelectModule,
    ],
})
export class UserProfileOverviewComponent extends FormBaseComponent<UpdateUserModel> {
    @Output() public setUserInfo = new EventEmitter<boolean>();
    public userInfo: ViewUserDetails;
    constructor(
        public _appConfig: AppConfig,
        private _userService: UserService,
        private _toastr: ToastrService,
        protected progressBarService: ProgressBarService
    ) {
        super(
            getGroup<UpdateUserModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    firstName: { vldtr: [Validators.required] },
                    lastName: { vldtr: [Validators.required] },
                    email: {
                        vldtr: [Validators.email]
                    },
                    phoneNumber: {
                        vldtr: [
                            Validators.minLength(10),
                            Validators.maxLength(10),
                        ],
                    },
                    roleName: { vldtr: [Validators.required] }
                }
            )
        );
    }

    protected save() {
        if (this.value === null) {
            return;
        }
        const userInfo: UpdateUserModel = {
            ...this.value,
        };
        this.userInfo.roleName = this.form.value.roleName;
        this._userService.updateMyProfile(userInfo).subscribe(
            (res) => {
                if (res && res.isSucceeded) {
                    this.setUserInfo.emit(true);
                    this._toastr.success(res.message);
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

    protected numberOnly(event): boolean {
        const charCode = event.which ? event.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
}