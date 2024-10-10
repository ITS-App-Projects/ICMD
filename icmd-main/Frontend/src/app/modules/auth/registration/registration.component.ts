import { Component, OnInit, OnDestroy } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators } from "@angular/forms";
import { Subscription, Observable } from "rxjs";
import { AuthService } from "../_services/auth.service";
import { Router } from "@angular/router";
import { ConfirmPasswordValidator } from "./confirm-password.validator";
import { first } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { RegisterRequestDto } from "@m/auth";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { AppRoute } from "@u/app.route";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-registration",
  templateUrl: "./registration.component.html",
  styleUrls: ["./registration.component.scss"],
})
export class RegistrationComponent implements OnInit, OnDestroy {
  registrationForm: UntypedFormGroup;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  protected route = AppRoute;
  protected message: string;

  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: UntypedFormBuilder,
    private authService: AuthService,
    private router: Router,
    private appConfig: AppConfig,
    private _toastr: ToastrService,
  ) {
    this.isLoading$ = this.authService.isLoading$;
    // redirect to home if already logged in
    if (appConfig.isAuthenticated()) {
      this.router.navigate(["/"]);
    } else
      this.appConfig.removeCurrentUser();
  }

  ngOnInit(): void {
    this.initForm();
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.registrationForm.controls;
  }

  initForm() {
    this.registrationForm = this.fb.group(
      {
        firstName: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ],
        lastName: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ],
        userName: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ],
        email: [
          "",
          Validators.compose([
            Validators.required,
            Validators.email,
            Validators.minLength(3),
            Validators.maxLength(320),
          ]),
        ],
        password: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ],
        cPassword: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ],
      },
      {
        validator: ConfirmPasswordValidator.MatchPassword,
      }
    );
  }

  submit() {
    this.hasError = false;
    const registerInfo: RegisterRequestDto = {
      userName: this.f.userName.value,
      password: this.f.password.value,
      email: this.f.email.value,
      firstName: this.f.firstName.value,
      lastName: this.f.lastName.value
    };
    const registrationSubscr = this.authService
      .register(registerInfo)
      .pipe(first())
      .subscribe((result: BaseResponseModel) => {
        if (result.isSucceeded) {
          this._toastr.success(result.message);
          this.router.navigate([AppRoute.login]);
        } else {
          this.message = result.message.replace(",", "<br>");
          this.hasError = true;
        }
      });
    this.unsubscribe.push(registrationSubscr);
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
