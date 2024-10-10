import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AppConfig } from "../app.config";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { EMPTY, Observable, throwError } from "rxjs";
import { catchError, finalize } from "rxjs/operators";
import { AuthorizeHandlerService, IdleUserService } from "../service/common";
import { AppRoute } from "@u/index";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    private _isNetworkErrorShown = false;
    private _isServerErrorShown = false;

    constructor(
        private _appConfig: AppConfig,
        private router: Router,
        private _toastr: ToastrService,
        private _idleService: IdleUserService,
        private _authorizeService: AuthorizeHandlerService
    ) { }
    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((err: HttpErrorResponse) => {
                // network issue
                if (err.status === 0) {
                    if (!this._isNetworkErrorShown) {
                        this._isNetworkErrorShown = true;
                        this._toastr
                            .error(
                                "Please, check the internet connection",
                                "Network Error"
                            )
                            .onHidden.subscribe(() => (this._isNetworkErrorShown = false));
                        this.router.navigate([""]);
                    }
                }

                //page Not Found
                else if (err.status === 404) {
                    this._toastr.error(
                        "Sorry, but we could not find the page you are looking for."
                    );
                    this.router.navigate([""]);
                }

                // unautorize error
                else if (err.status === 401 || err.status === 403) {
                    debugger
                    console.log("Error Code 401");
                    var isAuthenticated = this._appConfig.isAuthenticated();
                    if (!isAuthenticated) {
                        if (this._idleService.isUserIdle()) {
                            console.log("User is idle.");
                            this._appConfig.removeUserAndSendToLogin(this.router);
                            return EMPTY;
                        } else {
                            return this._authorizeService.handle401Error(req, next);
                        }
                    } else {
                        this._toastr.error(
                            "You are not authorized to view this page."
                        );
                        this.router.navigate([AppRoute.dashboard]);
                        //this._appConfig.removeUserAndSendToLogin(this.router);
                    }
                }

                //not acceptable or jwt token is not valid
                else if (err.status === 406) {
                    const isAuthenticated = this._appConfig.isAuthenticated();
                    if (isAuthenticated) {
                        this._appConfig.removeUserAndSendToLogin(this.router);
                    } else this.router.navigate([""]);
                }

                // other errors
                else if (err.status && err.status >= 400) {
                    if (req.method === "GET" && !this._isServerErrorShown) {
                        this._isServerErrorShown = true;
                        this._toastr
                            .error(
                                "looks like we have an internal issue, please try again in a couple of minutes."
                            )
                            .onHidden.subscribe(() => (this._isServerErrorShown = false));
                        this.router.navigate([""]);
                    }
                    else {
                        this._toastr.error(err?.error?.message, "", {
                            enableHtml: true,
                        });
                    }
                }
                return throwError(err);
            }),
            finalize(() => {
            })
        );
    }
}