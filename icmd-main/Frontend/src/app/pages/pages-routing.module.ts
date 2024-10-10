import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from '../_layout/layout.component';
import { AppRoute } from '@u/app.route';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: AppRoute.dashboard,
        loadChildren: () =>
          import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
      },
      {
        path: AppRoute.manageUser,
        loadChildren: () =>
          import("./admin/manage-users/manage-users.module").then((m) => m.ManageUsersModule),
      },
      {
        path: AppRoute.manageProject,
        loadChildren: () =>
          import("./admin/manage-projects/manage-projects.module").then((m) => m.ManageProjectsModule),
      },
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full',
      },

      //Master Menu
      {
        path: "",
        loadChildren: () =>
          import("./admin/masters/masters.module").then((m) => m.MastersModule),
      },

      //Instrument-List
      {
        path: AppRoute.instrumentList,
        loadChildren: () =>
          import("./admin/instrument-list/instrument-list.module").then((m) => m.InstrumentListModule),
      },

      //Non-Instrument-List
      {
        path: AppRoute.nonInstrumentList,
        loadChildren: () =>
          import("./admin/nonInstrument-list/nonInstrument-list.module").then((m) => m.NonInstrumentListModule),
      },

      //Device
      {
        path: AppRoute.manageDevice + "/:deviceId",
        loadChildren: () =>
          import("./admin/manage-device/manage-device.module").then((m) => m.ManageDeviceModule),
      },

      //Device
      {
        path: AppRoute.viewDevice + "/:deviceId",
        loadChildren: () =>
          import("./admin/manage-device/view-device-page/view-device-page.module").then((m) => m.ViewDevicePageModule),
      },

      //Hierarchy
      {
        path: AppRoute.manageHierarchy,
        loadChildren: () =>
          import("./admin/hierarchy/hierarchy.module").then((m) => m.HierarchyModule),
      },
      //Logs
      {
        path: AppRoute.manageLogs,
        loadChildren: () =>
          import("./admin/manage-logs/manage-logs.module").then((m) => m.ManageLogsModule),
      },

      //Import
      {
        path: AppRoute.manageImport,
        loadChildren: () =>
          import("./admin/manage-import/manage-import.module").then((m) => m.ManageImportModule),
      },

      //Report
      {
        path: AppRoute.manageReports,
        loadChildren: () =>
          import("./admin/manage-reports/manage-reports.module").then((m) => m.ManageReportsModule),
      },
      //View Report
      {
        path: AppRoute.viewReport + "/:name",
        loadChildren: () =>
          import("./admin/manage-reports/view-report-page/view-report-page.module").then((m) => m.ViewReportPageModule),
      },
      {
        path: "",
        loadChildren: () =>
          import("./admin/menu-role/menu-role.module").then(
            (m) => m.MenuRoleModule
          ),
      },
      //Permission Menu
      {
        path: "",
        loadChildren: () =>
          import("./admin/permission/permission.module").then(
            (m) => m.PermissionModule
          ),
      },
      {
        path: AppRoute.userProfile,
        loadChildren: () =>
          import(
            "./admin/manage-users/user-profile-page/user-profile-page.module"
          ).then((m) => m.UserProfilePageModule),
      },

      {
        path: AppRoute.changePassword,
        loadChildren: () =>
          import(
            "./admin/manage-users/user-profile-page/user-profile-page.module"
          ).then((m) => m.UserProfilePageModule),
      },


      ////
      {
        path: 'builder',
        loadChildren: () =>
          import('./builder/builder.module').then((m) => m.BuilderModule),
      },
      {
        path: 'ecommerce',
        loadChildren: () =>
          import('../modules/e-commerce/e-commerce.module').then(
            (m) => m.ECommerceModule
          ),
      },
      {
        path: 'user-management',
        loadChildren: () =>
          import('../modules/user-management/user-management.module').then(
            (m) => m.UserManagementModule
          ),
      },
      {
        path: 'user-profile',
        loadChildren: () =>
          import('../modules/user-profile/user-profile.module').then(
            (m) => m.UserProfileModule
          ),
      },
      {
        path: 'ngbootstrap',
        loadChildren: () =>
          import('../modules/ngbootstrap/ngbootstrap.module').then(
            (m) => m.NgbootstrapModule
          ),
      },
      {
        path: 'wizards',
        loadChildren: () =>
          import('../modules/wizards/wizards.module').then(
            (m) => m.WizardsModule
          ),
      },
      {
        path: 'material',
        loadChildren: () =>
          import('../modules/material/material.module').then(
            (m) => m.MaterialModule
          ),
      },
      {
        path: '**',
        redirectTo: 'error/404',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule { }
