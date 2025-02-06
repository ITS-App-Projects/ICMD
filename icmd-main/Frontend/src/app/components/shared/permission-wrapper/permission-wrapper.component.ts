import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  Output
} from '@angular/core';
import {
  AuthorizationType,
  RoleEnum
} from '@e/common';
import { isPermissionGranted } from '@u/helpers';

@Component({
    standalone: true,
    selector: "permission-wrapper",
    templateUrl: "./permission-wrapper.component.html",
    imports: [CommonModule],
})
export class PermissionWrapperComponent implements OnDestroy {
    @Output() public hasNotPermission = new EventEmitter<boolean>(false);
    protected hasPermission: boolean = false;
    private _currentUserPermissions: string[] = [];

    private _destroy$: Subject<void> = new Subject<void>();

    constructor(private _appConfig: AppConfig, private _cd: ChangeDetectorRef) {
        this._currentUserPermissions = this._appConfig
            .getCurrentUserPermission()
            .find(
                (s) => "/" + s.url == this._appConfig.currentActiveMenu
            )?.permissionName;
    }

    @Input() public set permissions(data: ReadonlyArray<string>) {
        if (this._currentUserPermissions) {

            if (this._appConfig.getCurrentUser().roleName != RoleEnum.Administrator) {
                this._appConfig.projectIdFilter$.pipe(takeUntil(this._destroy$)).subscribe((res) => {
                    if (res) {
                        let authorizationType = res.authorization ?? null;

                        if (authorizationType == AuthorizationType.ReadOnly)
                            this.hasPermission = false;
                        else if (authorizationType == AuthorizationType.ReadWrite) {
                            this.hasPermission = isPermissionGranted(this._currentUserPermissions, data);
                        } else if (authorizationType == AuthorizationType.Administrator)
                            this.hasPermission = true;
                    }
                    this._cd.detectChanges();
                    if (!this.hasPermission) this.hasNotPermission.emit(true);
                });
            } else {
                this.hasPermission = isPermissionGranted(this._currentUserPermissions, data);
                if (!this.hasPermission) this.hasNotPermission.emit(true);
            }
        }
        else {
            this.hasPermission = false;
            this.hasNotPermission.emit(true);
        }
    }

    @Input() public set permissionByRole(roles: ReadonlyArray<RoleEnum>) {
        this.hasPermission = false;
        roles.forEach(x => {
             if (x == this._appConfig.getCurrentUser().roleName)
                this.hasPermission = true;
         });
         this._cd.detectChanges();
         if (!this.hasPermission) this.hasNotPermission.emit(true);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }

    checkPermission(): boolean {
        return this.hasPermission;
    }
}
