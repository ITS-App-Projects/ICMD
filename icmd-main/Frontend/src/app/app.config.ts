import { Router, RouterStateSnapshot } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { OperationEnum } from "@e/common";
import { LoggedInUser } from "@m/auth";
import { DropdownInfoDtoModel } from "@m/common";
import { MenuAndPermissionListDto, MenuItemListModel, UserPermissionModel } from "@m/menu";
import { decodedTokenConstants } from "@u/constants";
import { BehaviorSubject } from "rxjs";
import { AppRoute } from "./utility";

export class AppConfig {
  private readonly currentUserTokenSessionKey: string = "currentUserToken";
  private readonly currentUserMenuPermissionSessionKey: string =
    "currentUserMenuPermission";
  private readonly currentUserIdSessionKey: string = "currentUserId";
  public projectIdFilter$: BehaviorSubject<DropdownInfoDtoModel> =
    new BehaviorSubject(null);
  public isProjectUpdate$: BehaviorSubject<boolean> =
    new BehaviorSubject(false);
  public isUpdateTagField$: BehaviorSubject<boolean> =
    new BehaviorSubject(false);
  public currentProjectId: string | null = null;
  public isPreviousURL$: string | null = null;
  public currentActiveMenu: string = "";

  public isRefreshingToken = false;

  public get Operations(): typeof OperationEnum {
    return OperationEnum;
  }

  public getToken() {
    const token = JSON.parse(
      String(localStorage.getItem(this.currentUserTokenSessionKey))
    );
    if (token) {
      return token;
    } else {
      return "";
    }
  }

  decodeToken() {
    const currentUserToken = JSON.parse(
      String(localStorage.getItem(this.currentUserTokenSessionKey))
    );
    if (currentUserToken !== null && currentUserToken !== undefined) {
      const helper = new JwtHelperService();
      const decodedToken = helper.decodeToken(currentUserToken);
      return decodedToken;
    }
    return null;
  }

  setToken(token: string) {
    localStorage.setItem(
      this.currentUserTokenSessionKey,
      JSON.stringify(token)
    );

    localStorage.setItem(
      this.currentUserIdSessionKey,
      btoa(this.getCurrentUserId())?.toString()
    );
  }

  removeToken() {
    localStorage.removeItem(this.currentUserTokenSessionKey);
  }

  getUserId() {
    return atob(
      localStorage.getItem(this.currentUserIdSessionKey)
    ) ?? null;
  }

  private getCurrentUserId() {
    const decodedToken = this.decodeToken();
    if (decodedToken) {
      const id = decodedToken[decodedTokenConstants.userId];
      return id;
    }
    return null;
  }

  public getCurrentUser(): LoggedInUser {
    const currentUserToken = JSON.parse(
      String(localStorage.getItem(this.currentUserTokenSessionKey))
    );
    if (currentUserToken !== null && currentUserToken !== undefined) {
      const helper = new JwtHelperService();
      const decodedToken = helper.decodeToken(currentUserToken);

      const currUser: LoggedInUser = {
        userId: decodedToken[decodedTokenConstants.userId],
        userName: decodedToken[decodedTokenConstants.userName],
        fullName: decodedToken[decodedTokenConstants.userFullName],
        email: decodedToken[decodedTokenConstants.email],
        roleName: decodedToken[decodedTokenConstants.roleName]
      };
      return currUser;
    } else {
      return null;
    }
  }

  public setCurrentUser(token: string, menuPermission: string) {
    localStorage.removeItem(this.currentUserTokenSessionKey);
    localStorage.removeItem(this.currentUserMenuPermissionSessionKey);
    localStorage.setItem(
      this.currentUserTokenSessionKey,
      JSON.stringify(token)
    );
    localStorage.setItem(
      this.currentUserIdSessionKey,
      btoa(this.getCurrentUserId())?.toString()
    );
    if (menuPermission) {
      localStorage.setItem(
        this.currentUserMenuPermissionSessionKey,
        btoa(menuPermission)?.toString()
      );
    }
  }

  public removeCurrentUser() {
    localStorage.removeItem(this.currentUserTokenSessionKey);
    localStorage.removeItem(this.currentUserMenuPermissionSessionKey);
    localStorage.removeItem(this.currentUserIdSessionKey);
  }

  public isAuthenticated(): boolean {
    const currentUserToken = JSON.parse(
      String(localStorage.getItem(this.currentUserTokenSessionKey))
    );
    if (currentUserToken !== null && currentUserToken !== undefined) {
      const helper = new JwtHelperService();
      const decodedToken = helper.decodeToken(currentUserToken);
      var currentDate = Math.floor(Date.now() / 1000);
      if (decodedToken["exp"] > currentDate) {
        return true;
      } else {
        //this.removeCurrentUser();
        return false;
      }
    } else {
      //this.removeCurrentUser();
      return false;
    }
  }

  public getCurrentProjectId() {
    return this.currentProjectId;
  }

  public getCurrentUserMenu(): MenuItemListModel[] {
    const menuAndPermission = this.getCurrentUserMenuAndPermissions();
    return menuAndPermission?.menuItems ?? [];
  }

  public getCurrentUserPermission(): UserPermissionModel[] {
    const menuAndPermission: MenuAndPermissionListDto =
      this.getCurrentUserMenuAndPermissions();
    return (menuAndPermission?.permissions as UserPermissionModel[]) ?? [];
  }

  public get getInactivityTimeout() {
    return 30 * 60 * 1000;
  }

  public removeUserAndSendToLogin(router: Router, sendToReturnUrl: boolean = true) {
    this.removeCurrentUser();
    const snapshot: RouterStateSnapshot = router.routerState.snapshot;
    router.navigate([AppRoute.login], { queryParams: { returnUrl: sendToReturnUrl ? snapshot.url : null } }).then(() => {
      window.location.reload();
    });
  }

  private getCurrentUserMenuAndPermissions(): MenuAndPermissionListDto {
    const decodedToken = this.decodeToken();
    if (decodedToken) {
      // const menuAndPermission =
      //   decodedToken[decodedTokenConstants.menuAndPermission];
      const menuAndPermission = atob(
        localStorage.getItem(this.currentUserMenuPermissionSessionKey)
      );

      return (
        (JSON.parse(menuAndPermission) as MenuAndPermissionListDto) ?? null
      );
    }

    return null;
  }
}
