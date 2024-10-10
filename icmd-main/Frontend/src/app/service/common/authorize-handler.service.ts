
import { HttpClient, HttpHandler, HttpRequest } from "@angular/common/http";
import { Injectable, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "@module/auth";
import { BehaviorSubject, EMPTY } from "rxjs";
import { filter, switchMap, take } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";


@Injectable({ providedIn: "root" })
export class AuthorizeHandlerService {
    private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
    private requestsQueue: HttpRequest<any>[] = [];

    constructor(private _http: HttpClient, private _appConfig: AppConfig, private _authservice: AuthService, private router: Router, private injector: Injector) { }
    //Token Handlers
    public handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        debugger;
        console.log("Handle 401");
        if (!this._appConfig.isRefreshingToken) {
            this._appConfig.isRefreshingToken = true;
            this._appConfig.removeToken();
            const userId = this._appConfig.getUserId() ?? null;

            if (userId && userId?.trim() != "") {
                this.requestsQueue.push(request);
                console.log("Refreshing token...");

                return this._authservice.refreshToken(userId).pipe(
                    switchMap((newToken: string | null) => {
                        debugger
                        this._appConfig.isRefreshingToken = false;
                        if (newToken && newToken != "") {
                            //this.refreshTokenSubject.complete();
                            if (this.requestsQueue.length > 1)
                                this.refreshTokenSubject.next(newToken);
                            else
                                this.retryStoredRequests();

                            //this.retryStoredRequests();
                            // this._appConfig.requestQueue.forEach(req => {
                            //     next.handle(req).subscribe();
                            // });
                            // // Clear the queue
                            // this._appConfig.requestQueue = [];
                            return EMPTY;
                        }
                        else
                            this._appConfig.removeUserAndSendToLogin(this.router);
                    })
                );
            } else {
                this._appConfig.isRefreshingToken = false;
                this._appConfig.removeUserAndSendToLogin(this.router);
            }
        }
        else {
            this.requestsQueue.push(request);
            return this.refreshTokenSubject.pipe(
                filter(token => token !== null),
                take(1),
                switchMap(() => {
                    this.retryStoredRequests();
                    this.refreshTokenSubject.complete();
                    // this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
                    //     this.router.navigate([this.router.url]);
                    // });
                    return EMPTY;
                })
            );
        }
    }

    private retryStoredRequests() {
        if (this.requestsQueue.length > 0)
            this.requestsQueue = [];
        location.reload();
    }
}
