import { Component, OnInit, OnDestroy, Input } from "@angular/core";
import { UntypedFormBuilder, UntypedFormGroup, Validators } from "@angular/forms";
import { Subscription, Observable } from "rxjs";
import { first } from "rxjs/operators";
import { AuthService } from "../_services/auth.service";
import { Router } from "@angular/router";
import * as CryptoJS from "crypto-js";
import { LoginRequestModel } from "src/app/models/auth/login-request-model";
import { LoginResponseDataModel, LoginResponseModel } from "src/app/models/auth/login-response-model";
import { AppConfig } from "src/app/app.config";
import { CookieService } from "ngx-cookie-service";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"],
})
export class LoginComponent implements OnInit, OnDestroy {
  isRememberMe: boolean = false;
  loginForm: UntypedFormGroup;
  hasError: boolean;
  @Input() returnUrl!: string;
  isLoading$: Observable<boolean>;
  protected message: string;

  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: UntypedFormBuilder,
    private authService: AuthService,
    private router: Router,
    private appConfig: AppConfig,
    private cookieService: CookieService
  ) {
    this.isLoading$ = this.authService.isLoading$;

    // redirect to home if already logged in
    if (this.appConfig.isAuthenticated()) {
      this.router.navigate(["/"]);
    } else
      this.appConfig.removeCurrentUser();
  }

  ngOnInit(): void {
    this.initForm();
     this.returnUrl = this.returnUrl ? this.returnUrl : "/";
    //   this.route.snapshot.queryParams["returnUrl".toString()] || "/";
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.loginForm.controls;
  }

  initForm() {
    this.loginForm = this.fb.group({
      userName: [
        "",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(320), // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
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
    });
  }

  rememberMe(event) {
    this.isRememberMe = event.checked;
  }

  submit() {
    this.hasError = false;
    const loginInfo: LoginRequestModel = {
      userName: this.f.userName.value,
      password: this.f.password.value,
      rememberMe: false,
    };
    const loginSubscr = this.authService
      .login(loginInfo)
      .pipe(first())
      .subscribe((result: LoginResponseModel) => {
        if (result && result.isSucceeded) {
          if (this.rememberMe) {
            this.setRememberMe(loginInfo);
          }
          this.loginSuccess(result.token, result?.data);
        } else {
          this.message = result.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(loginSubscr);
  }

  loginSuccess(result: string, data: LoginResponseDataModel) {
    this.appConfig.setCurrentUser(result, data?.menuPermission);
    this.router.navigate([this.returnUrl]);
  }

  setRememberMe(model) {
    var token = CryptoJS.AES.encrypt(model.password, model.userName).toString();
    var rememberMe = {
      userName: model.userName,
      token: token,
    };

    var todayDate = new Date();
    todayDate.setDate(todayDate.getMonth() + 12);

    this.cookieService.delete("_rememberMe", "/");
    this.cookieService.set(
      "_rememberMe",
      JSON.stringify(rememberMe),
      todayDate,
      "/"
    );
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
