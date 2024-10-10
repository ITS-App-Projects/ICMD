import { Component, OnInit, AfterViewInit } from "@angular/core";
import KTLayoutQuickSearch from "../../../../assets/js/layout/extended/quick-search";
import KTLayoutQuickNotifications from "../../../../assets/js/layout/extended/quick-notifications";
import KTLayoutQuickActions from "../../../../assets/js/layout/extended/quick-actions";
import KTLayoutQuickCartPanel from "../../../..//assets/js/layout/extended/quick-cart";
import KTLayoutQuickPanel from "../../../../assets/js/layout/extended/quick-panel";
import KTLayoutQuickUser from "../../../../assets/js/layout/extended/quick-user";
import KTLayoutHeaderTopbar from "../../../../assets/js/layout/base/header-topbar";
import { KTUtil } from "../../../../assets/js/components/util";
import { AppConfig } from "src/app/app.config";
import { LayoutService } from "src/app/_metronic/core";
import { LoggedInUser } from "@m/auth";
import { ProjectService } from "src/app/service/manage-projects";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { DropdownInfoDtoModel } from "@m/common";
import { UserService } from "src/app/service/manage-user";
import { Router } from "@angular/router";
import { RoleEnum } from "@e/common";

@Component({
  selector: "app-topbar",
  templateUrl: "./topbar.component.html",
  styleUrls: ["./topbar.component.scss"],
})
export class TopbarComponent implements OnInit, AfterViewInit {
  user$: LoggedInUser;
  // tobbar extras
  extraSearchDisplay: boolean;
  extrasSearchLayout: "offcanvas" | "dropdown";
  extrasNotificationsDisplay: boolean;
  extrasNotificationsLayout: "offcanvas" | "dropdown";
  extrasQuickActionsDisplay: boolean;
  extrasQuickActionsLayout: "offcanvas" | "dropdown";
  extrasCartDisplay: boolean;
  extrasCartLayout: "offcanvas" | "dropdown";
  extrasQuickPanelDisplay: boolean;
  extrasLanguagesDisplay: boolean;
  extrasUserDisplay: boolean;
  extrasUserLayout: "offcanvas" | "dropdown";
  protected projectsData: DropdownInfoDtoModel[] = [];
  protected projectId: string = null;
  private _destroy$ = new Subject<void>();

  constructor(private layout: LayoutService, private appConfig: AppConfig, private _projectService: ProjectService,
    private _userService: UserService, private router: Router) {
    this.user$ = this.appConfig.getCurrentUser();
    this.getAllProjectsInfo();
  }

  ngOnInit(): void {
    // topbar extras
    this.extraSearchDisplay = this.layout.getProp("extras.search.display");
    this.extrasSearchLayout = this.layout.getProp("extras.search.layout");
    this.extrasNotificationsDisplay = this.layout.getProp(
      "extras.notifications.display"
    );
    this.extrasNotificationsLayout = this.layout.getProp(
      "extras.notifications.layout"
    );
    this.extrasQuickActionsDisplay = this.layout.getProp(
      "extras.quickActions.display"
    );
    this.extrasQuickActionsLayout = this.layout.getProp(
      "extras.quickActions.layout"
    );
    this.extrasCartDisplay = this.layout.getProp("extras.cart.display");
    this.extrasCartLayout = this.layout.getProp("extras.cart.layout");
    this.extrasLanguagesDisplay = this.layout.getProp(
      "extras.languages.display"
    );
    this.extrasUserDisplay = this.layout.getProp("extras.user.display");
    this.extrasUserLayout = this.layout.getProp("extras.user.layout");
    this.extrasQuickPanelDisplay = this.layout.getProp(
      "extras.quickPanel.display"
    );
  }

  ngAfterViewInit(): void {
    KTUtil.ready(() => {
      // Called after ngAfterContentInit when the component's view has been initialized. Applies to components only.
      // Add 'implements AfterViewInit' to the class.
      if (this.extraSearchDisplay && this.extrasSearchLayout === "offcanvas") {
        KTLayoutQuickSearch.init("kt_quick_search");
      }

      if (
        this.extrasNotificationsDisplay &&
        this.extrasNotificationsLayout === "offcanvas"
      ) {
        // Init Quick Notifications Offcanvas Panel
        KTLayoutQuickNotifications.init("kt_quick_notifications");
      }

      if (
        this.extrasQuickActionsDisplay &&
        this.extrasQuickActionsLayout === "offcanvas"
      ) {
        // Init Quick Actions Offcanvas Panel
        KTLayoutQuickActions.init("kt_quick_actions");
      }

      if (this.extrasCartDisplay && this.extrasCartLayout === "offcanvas") {
        // Init Quick Cart Panel
        KTLayoutQuickCartPanel.init("kt_quick_cart");
      }

      if (this.extrasQuickPanelDisplay) {
        // Init Quick Offcanvas Panel
        KTLayoutQuickPanel.init("kt_quick_panel");
      }

      if (this.extrasUserDisplay && this.extrasUserLayout === "offcanvas") {
        // Init Quick User Panel
        KTLayoutQuickUser.init("kt_quick_user");
      }

      // Init Header Topbar For Mobile Mode
      KTLayoutHeaderTopbar.init("kt_header_mobile_topbar_toggle");
    });

    this.appConfig.isProjectUpdate$.subscribe((res) => {
      if (res) {
        this.getAllProjectsInfo();
      }
    })
  }

  protected onChangeProject(id: string): void {
    this._userService.updateUserProject(id)
      .pipe(takeUntil(this._destroy$))
      .subscribe((res) => {
        if (res) {
          this.projectId = id;
          this.appConfig.projectIdFilter$.next(this.projectsData.find(s => s.id == id));
          this.appConfig.currentProjectId = id;
        }
      });
  }

  private getAllProjectsInfo(): void {
    this._projectService.getAllProjectsInfo()
      .pipe(takeUntil(this._destroy$))
      .subscribe((res) => {
        this.projectsData = res;
        this.projectId = res.length != 0 ? res[0].id : null;
        this.appConfig.currentProjectId = this.projectId;
        this.appConfig.projectIdFilter$.next(res.length != 0 ? res[0] ?? null : null);
        if (res && res.length == 0 && this.user$.roleName != RoleEnum.Administrator)
          this.router.navigate(["error/project-error"]);
      });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }
}
