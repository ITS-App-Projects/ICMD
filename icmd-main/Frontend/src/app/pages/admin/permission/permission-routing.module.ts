import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AppRoute } from "@u/app.route";

const routes: Routes = [
  {
    path: "",
    canActivate: [],
    children: [
      {
        path: AppRoute.permissionManangment,
        pathMatch: "full",
        loadChildren: () =>
          import(
            "./permission-page/permission-page.module"
          ).then((m) => m.PermissionManagementPageModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [],
  providers: [],
})
export class PermissionRoutingModule {}
