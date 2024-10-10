import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ProgressBarService } from "../service/common";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
    private totalRequests = 0;
    constructor(private _progressBarService: ProgressBarService) { }
    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        this.totalRequests++;
        this._progressBarService.isLoading$.next(true);

        return next.handle(request).pipe(
            finalize(() => {
                this.totalRequests--;
                if (this.totalRequests == 0) {
                    this._progressBarService.isLoading$.next(false);
                }
            })
        );
    }
}