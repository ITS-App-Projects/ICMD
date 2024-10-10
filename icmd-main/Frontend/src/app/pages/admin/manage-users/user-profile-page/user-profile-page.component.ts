import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { ListUserTableComponent } from "@c/manange-user/list-user-table";
import { FormDefaultsModule } from "@c/shared/forms";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { UserProfileOverviewComponent } from "../user-profile-overview";
import { LoggedInUser } from "@m/auth";
import { AppConfig } from "src/app/app.config";
import { UserService } from "src/app/service/manage-user";
import { ViewUserDetails } from "@c/manange-user/create-edit-user-form";
import { Router } from "@angular/router";
import { AppRoute } from "@u/app.route";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { UserChangePasswordComponent } from "../user-change-password";

@Component({
    standalone: true,
    selector: "app-user-profile-page",
    templateUrl: "./user-profile-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatFormFieldModule,
        ListUserTableComponent,
        MatDialogModule,
        MatInputModule,
        ReactiveFormsModule,
        MatSelectModule,
        MatExpansionModule,
        PermissionWrapperComponent,
        UserProfileOverviewComponent,
        UserChangePasswordComponent
    ],
    providers: [
    ]
})
export class UserProfilePageComponent {
    @ViewChild(UserProfileOverviewComponent)
    profileOverview: UserProfileOverviewComponent;
    protected user: LoggedInUser;
    protected userInfo: ViewUserDetails;
    protected tabId: number = 0;
    private _destroy$ = new Subject<void>();

    constructor(
        public _appConfig: AppConfig,
        private _userService: UserService,
        private _cdr: ChangeDetectorRef,
        private _router: Router
    ) {
        this.user = this._appConfig.getCurrentUser();
    }

    ngOnInit() { }

    ngAfterViewInit() {
        if (this.user.userId != null) {
            this.getUserInfo();
        }

        if (this._router.url.includes(AppRoute.changePassword)) this.tabId = 1;
        else this.tabId = 0;
        if (this.user.userId != null) {
            this.getUserInfo();
        }
    }

    protected getUserInfo() {
        this._userService.getUserInfo(this.user.userId).pipe(takeUntil(this._destroy$)).subscribe((res) => {
            this.userInfo = res;
            if (this.profileOverview) {
                this.profileOverview.userInfo = res;
                this.profileOverview.form.patchValue({
                    id: res?.id,
                    firstName: res?.firstName,
                    lastName: res?.lastName,
                    email: res?.email,
                    phoneNumber: res?.phoneNumber,
                    roleName: res?.roleName
                });
            }

            this._cdr.detectChanges();
        });
    }

    protected setTabIndex(id: number) {
        if (id == 0) this.getUserInfo();
        this.tabId = id;
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}