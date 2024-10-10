import { Injectable } from "@angular/core";
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { AppConfig } from "src/app/app.config";
import { IdleUserService } from "src/app/service/common";

@Injectable({ providedIn: "root" })
export class AuthGuard  {
  constructor(
    private router: Router,
    private readonly appConfig: AppConfig,
    private _idleService: IdleUserService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    // if (this.appConfig.isAuthenticated()) {
    //   return true;
    // } else {
    //   debugger
    //   this.appConfig.removeCurrentUser();
    // }

    //AuthGuard is not currently in use.
    if (!this.appConfig.isAuthenticated()) {
      if (this._idleService.isUserIdle()) {
        this.appConfig.removeUserAndSendToLogin(this.router);
      } else
        return true;
    } else 
      return true;

    return false;
    // const currentUser = this.authService.currentUserValue;
    // if (currentUser) {
    //   // logged in so return true
    //   return true;
    // }
    // // not logged in so redirect to login page with the return url
    // this.authService.logout();
    // return false;
  }
}
