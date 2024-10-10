import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AppRoute } from "@u/app.route";

const routes: Routes = [
    {
        path: "",
        canActivate: [],
        children: [
            {
                path: AppRoute.menuManangement,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import("./menu-list-page/menu-list-page.module").then(
                        (m) => m.MenuListPageModule
                    ),
            },
            {
                path: AppRoute.roleManangement,
                pathMatch: "full",
                loadChildren: () =>
                    import("./role-management-page/role-management-page.module").then(
                        (m) => m.RoleManagementPageModule
                    ),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class MenuRoleRoutingModule { }
