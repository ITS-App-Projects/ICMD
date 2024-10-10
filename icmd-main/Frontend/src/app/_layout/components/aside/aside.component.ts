import { Location } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { GetProjectTagFieldNames } from '@m/common';
import { MenuItemListModel } from '@m/menu';
import { AppRoute } from '@u/index';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { LayoutService } from 'src/app/_metronic/core/services/layout.service';
import { AppConfig } from 'src/app/app.config';
import { ProjectService } from 'src/app/service/manage-projects';


@Component({
  selector: 'app-aside',
  templateUrl: './aside.component.html',
  styleUrls: ['./aside.component.scss'],
})
export class AsideComponent implements OnInit {
  disableAsideSelfDisplay: boolean;
  headerLogo: string;
  brandSkin: string;
  ulCSSClasses: string;
  location: Location;
  asideMenuHTMLAttributes: any = {};
  asideMenuCSSClasses: string;
  asideMenuDropdown;
  brandClasses: string;
  asideMenuScroll = 1;
  asideSelfMinimizeToggle = false;
  protected tagFieldNames: GetProjectTagFieldNames[] = [];
  private _destroy$ = new Subject<void>();
  private tagFieldRoutes: string[] = [AppRoute.manageProcess, AppRoute.manageSubProcess, AppRoute.manageStream];
  protected menuPermissionList: MenuItemListModel[];
  private processId: string | null = null;
  private subProcessId: string | null = null;
  private streamId: string | null = null;

  constructor(private layout: LayoutService, private loc: Location, private appConfig: AppConfig,
    private _projectService: ProjectService, private _cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    // load view settings
    this.disableAsideSelfDisplay =
      this.layout.getProp('aside.self.display') === false;
    this.brandSkin = this.layout.getProp('brand.self.theme');
    this.headerLogo = this.getLogo();
    this.ulCSSClasses = this.layout.getProp('aside_menu_nav');
    this.asideMenuCSSClasses = this.layout.getStringCSSClasses('aside_menu');
    this.asideMenuHTMLAttributes = this.layout.getHTMLAttributes('aside_menu');
    this.asideMenuDropdown = this.layout.getProp('aside.menu.dropdown') ? '1' : '0';
    this.brandClasses = this.layout.getProp('brand');
    this.asideSelfMinimizeToggle = this.layout.getProp(
      'aside.self.minimize.toggle'
    );
    this.asideMenuScroll = this.layout.getProp('aside.menu.scroll') ? 1 : 0;
    // this.asideMenuCSSClasses = `${this.asideMenuCSSClasses} ${this.asideMenuScroll === 1 ? 'scroll my-4 ps ps--active-y' : ''}`;
    // Routing
    this.location = this.loc;
    this.getAllSideMenu();
  }

  ngAfterViewInit(): void {
    this.appConfig.projectIdFilter$.subscribe((res) => {
      if (res) {
        this.getProjectTagFieldName(res?.id ?? null);
      }
    })
    this.appConfig.isProjectUpdate$.subscribe((res) => {
      if (res) {
        this.tagFieldNames = [];
      }
    });
    this.appConfig.isUpdateTagField$.subscribe((res) => {
      if (res) {
        this.getProjectTagFieldName(this.appConfig.currentProjectId ?? null);
      }
    });

  }

  protected getProjectTagFieldName(projectId: string): void {
    if (projectId) {
      this._projectService.getProjectTagFieldNames(projectId)
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
          this.tagFieldNames = [];
          res.map((item, index) => {
            this.tagFieldNames.push({ name: item, route: this.tagFieldRoutes[index] });
          })
          this.updateTagFieldName();
          this._cdr.detectChanges();
        });
    }
  }

  private getLogo() {
    if (this.brandSkin === 'light') {
      return './assets/media/logos/logo-dark.png';
    } else {
      return './assets/media/logos/logo-light.png';
    }
  }

  private getAllSideMenu() {
    const menus: MenuItemListModel[] = this.appConfig.getCurrentUserMenu();
    this.menuPermissionList = menus ?? null;
  }

  private updateTagFieldName(): void {
    if (this.tagFieldNames) {
      const processMenu = this.menuPermissionList.find(menuItem => menuItem.menuDescription === 'Project Masters');

      if (processMenu) {
        const tagFieldProcessSubMenu = this.processId ? processMenu.subMenu.find(subMenu => subMenu.id === this.processId)
          : processMenu.subMenu.find(subMenu => subMenu.menuDescription === 'Tag Field (Process)');
        const tagFieldSubProcessSubMenu = this.subProcessId ? processMenu.subMenu.find(subMenu => subMenu.id === this.subProcessId)
          : processMenu.subMenu.find(subMenu => subMenu.menuDescription === 'Tag Field (Sub Process)');
        const tagFieldStreamSubMenu = this.streamId ? processMenu.subMenu.find(subMenu => subMenu.id === this.streamId)
          : processMenu.subMenu.find(subMenu => subMenu.menuDescription === 'Tag Field (Stream)');

        if (tagFieldProcessSubMenu) {
          this.processId = tagFieldProcessSubMenu?.id ?? null;
          tagFieldProcessSubMenu.menuDescription = "Tag Field (" + (this.tagFieldNames[0]?.name ?? "") + ")";
        }

        if (tagFieldSubProcessSubMenu) {
          this.subProcessId = tagFieldSubProcessSubMenu?.id ?? null;
          tagFieldSubProcessSubMenu.menuDescription = "Tag Field (" + (this.tagFieldNames[1]?.name ?? "") + ")";
        }

        if (tagFieldStreamSubMenu) {
          this.streamId = tagFieldStreamSubMenu?.id ?? null;
          tagFieldStreamSubMenu.menuDescription = "Tag Field (" + (this.tagFieldNames[2]?.name ?? "") + ")";
        }
      }
    }
    this._cdr.detectChanges();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }
}
