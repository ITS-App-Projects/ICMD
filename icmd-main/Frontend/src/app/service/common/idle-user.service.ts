import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { fromEvent, merge } from "rxjs";
import { AppConfig } from "src/app/app.config";

@Injectable({ providedIn: 'root' })
export class IdleUserService {
    private timer: any;
    private lastActivityTime: number;
    keyboardInput$ = merge(fromEvent(window, 'click'), fromEvent(window, 'keypress'), fromEvent(window, 'scroll'))

    constructor(private _appConfig: AppConfig, private router: Router) {
        this.keyboardInput$?.subscribe(res => {
            clearTimeout(this.timer);
            if (this._appConfig.isAuthenticated()) {
                if (this.isUserIdle(false))
                    this._appConfig.removeUserAndSendToLogin(this.router);

                this.lastActivityTime = Date.now();
            }
        })
    }

    public isUserIdle(isFromInterceptor: boolean = true): boolean {
        const currentTime = Date.now();
        const elapsedTime = currentTime - this.lastActivityTime;

        if (isNaN(elapsedTime) && isFromInterceptor)
            return true;

        return elapsedTime > this._appConfig.getInactivityTimeout;
    }
}