import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Output,
} from "@angular/core";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import {
  CreateOrEditUserModel
} from "./create-edit-user-form.model";
import { getGroup } from "@u/forms";
import { Validators } from "@angular/forms";
import { MatIconModule } from "@angular/material/icon";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { RoleEnum } from "@e/common";
import { LoggedInUser } from "@m/auth";
import { AppConfig } from "src/app/app.config";
import { PasswordStrengthValidator, mustMatch } from "@u/validators";
import { RoleDetailsModel } from "@m/common/role-details.model";
import { MatButtonModule } from "@angular/material/button";

@Component({
  standalone: true,
  selector: "app-create-edit-user-form",
  templateUrl: "./create-edit-user-form.component.html",
  imports: [
    FormDefaultsModule,
    MatIconModule,
    MatButtonModule,],
  providers: [
  ],
})
export class CreateOrEditUserFormComponent extends FormBaseComponent<CreateOrEditUserModel> {
  @Output() public isDeleteProfileImage = new EventEmitter<string>();
  protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";
  private _destroy$ = new Subject<void>();
  public roleInfo: RoleDetailsModel[] = [];
  protected currentUserInfo: LoggedInUser;
  protected roleEnumInfo = RoleEnum;
  constructor(
    private _cdr: ChangeDetectorRef,
    private _appConfig: AppConfig
  ) {
    super(
      getGroup<CreateOrEditUserModel>(
        {
          id: { v: "00000000-0000-0000-0000-000000000000" },
          firstName: { vldtr: [Validators.required] },
          lastName: { vldtr: [Validators.required] },
          userName: { vldtr: [Validators.required] },
          email: {
            vldtr:[
              Validators.email
            ]
          },
          phoneNumber: {
            vldtr: [
              Validators.minLength(10),
              Validators.maxLength(10),
            ],
          },
          roleName: { vldtr: [Validators.required] },
          password: {
            vldtr: [
              Validators.required,
              Validators.minLength(8),
              PasswordStrengthValidator,
            ],
          },
          confirmPassword: { vldtr: [Validators.required] }
        },
        mustMatch("password", "confirmPassword", true)
      )
    );
    this.currentUserInfo = this._appConfig.getCurrentUser();
  }

  ngOnInit(): void { }

  protected numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
}
