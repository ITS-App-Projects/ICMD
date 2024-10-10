import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AppConfig } from "../app.config";
import { MenuItemListModel, UserPermissionModel } from "@m/menu";
import { Observable, of } from "rxjs";
import { permissionMenuName, requiredOperation } from "@u/default";
import { isPermissionGranted } from "@u/helpers";

@Injectable()
export class PermissionGuard  {
    private _currentUserPermissions: UserPermissionModel[] = [];

    constructor(
        protected router: Router,
        private _toastr: ToastrService,
        private _appConfig: AppConfig
    ) {
        this._currentUserPermissions = this._appConfig.getCurrentUserPermission();
    }

    canActivateChild(
        childRoute: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | Observable<boolean> | Promise<boolean> {
        return this.canActivate(childRoute, state);
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<boolean> {
        // if (!route.data || !route.data[permissionMenuName]) {
        //     return of(true);
        // }
        const isDirectlyOpen: boolean = !document.referrer;
        //const menus = route.data[permissionMenuName] as string;
        const operations = route.data[requiredOperation] as ReadonlyArray<string>;
        const userMenu: MenuItemListModel[] = this._appConfig.getCurrentUserMenu();


        const isUrlExist = userMenu.some(menuItem => this.checkUrlInMenu(menuItem, state?.url));

        const permissions = this._currentUserPermissions.find(
            (s) => ("/" + s.url) == state?.url
        )?.permissionName;

        if (!isUrlExist) {
            this.router.navigate([""]);
            this._toastr.error(
                "User have no permission to navigate the requested page."
            );
            return of(false);
        }


        const hasPermission = true;
        if (operations) {
            const hasPermission = isPermissionGranted(permissions, operations);
            if (!hasPermission) {
                this.router.navigate([""]);
                this._toastr.error(
                    "User have no permission to navigate the requested page."
                );
            }
            return of(hasPermission);
        }


        if (isDirectlyOpen && !hasPermission) {
            this.router.navigate([""]);
            return of(false);
        }

        if (isUrlExist) {
            this._appConfig.currentActiveMenu = state?.url;
            return of(true);
        }

    }

    checkUrlInMenu(menu: MenuItemListModel, urlToCheck: string): boolean {
        if (("/" + menu.url) === urlToCheck) {
            return true;
        }

        if (menu.subMenu && menu.subMenu.length > 0) {
            // Check submenus recursively
            return menu.subMenu.some(subMenu => ("/" + subMenu.url) === urlToCheck);
        }

        return false;
    }
}
