import { Injectable, OnDestroy } from "@angular/core";
import { Observable, BehaviorSubject, of, Subscription } from "rxjs";
import { map, catchError, finalize } from "rxjs/operators";
import { AuthHTTPService } from "./auth-http";
import { environment } from "src/environments/environment";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import {
  BaseResponseModel,
  LoggedInUserDtoModel,
  LoginResponseModel,
} from "src/app/models/auth/login-response-model";
import { LoginRequestModel } from "src/app/models/auth/login-request-model";
import { AppConfig } from "src/app/app.config";
import { RegisterRequestDto } from "@m/auth";
import { AppRoute } from "@u/app.route";

@Injectable({
  providedIn: "root",
})
export class AuthService implements OnDestroy {
  // private fields
  private _authenticated: boolean = false;
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  // public fields
  currentUser$: Observable<LoggedInUserDtoModel>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<LoggedInUserDtoModel>;
  isLoadingSubject: BehaviorSubject<boolean>;

  get currentUserValue(): LoggedInUserDtoModel {
    //return this.currentUserSubject.value;
    let info = JSON.parse(sessionStorage.getItem("currentUser"));
    this.currentUserValue = info;
    return info;
  }

  set currentUserValue(user: LoggedInUserDtoModel) {
    sessionStorage.setItem("currentUser", JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  constructor(
    private authHttpService: AuthHTTPService,
    private appConfig: AppConfig,
    private router: Router,
    private _http: HttpClient
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<LoggedInUserDtoModel>(
      undefined
    );
    //this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();
    //const subscr = this.getUserByToken().subscribe();
    //this.unsubscribe.push(subscr);
  }

  login(loginInfo: LoginRequestModel): Observable<LoginResponseModel> {
    this.isLoadingSubject.next(true);
    return this._http
      .post<LoginResponseModel>(`${environment.apiUrl}Account/Login`, loginInfo)
      .pipe(
        // map((auth: LoginResponseModel) => {
        //   if (auth.isSucceeded) {
        //     this.accessToken = auth.token;
        //     this._authenticated = true;
        //     this.currentUserValue = auth.userDto;
        //   }
        //   //const result = this.setAuthFromLocalStorage(auth);

        //   //   if (this.rememberMe) {
        //   //     this.setRememberMe(loginModel);
        //   // }
        //   return auth;
        // }),
        // switchMap(() => this.getUserByToken()),
        catchError((err) => {
          console.error("err", err);
          return of(undefined);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  register(registerInfo: RegisterRequestDto): Observable<BaseResponseModel> {
    this.isLoadingSubject.next(true);
    return this._http
      .post<BaseResponseModel>(`${environment.apiUrl}Account/Register`, registerInfo)
      .pipe(
        catchError((err) => {
          console.error("err", err);
          return of(undefined);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  logout() {
    this.isLoadingSubject.next(true);
    return this._http.get(`${environment.apiUrl}Account/Logout`).pipe(
      map((auth) => {
        //this.appConfig.removeCurrentUser();
        //this.router.navigate([AppRoute.login]);
        this.appConfig.removeUserAndSendToLogin(this.router);
      }),
      catchError((err) => {
        console.error("err", err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  forgotPassword(email: string): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .forgotPassword(email)
      .pipe(finalize(() => this.isLoadingSubject.next(false)));
  }

  refreshToken(userId: string): Observable<string | null> {
    this.isLoadingSubject.next(true);
    return this._http.get(`${environment.apiUrl}Account/RefreshToken?id=${userId}`).pipe(
      map((newToken: string | null) => {
        const hasNewToken = newToken && newToken?.trim() !== "";
        if (hasNewToken) {
          this.appConfig.setToken(newToken);
        }

        console.log("User Refreshed");
        return newToken;
      }),
      catchError((err) => {
        console.error("err", err.error);
        return of(null);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
