import { Route, RouterModule } from "@angular/router";
import { RoleManagementPageComponent } from "./role-management-page.component";
import { NgModule } from "@angular/core";

const userRoutes: Route[] = [
    {
        path: "",
        component: RoleManagementPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(userRoutes),
    ],
    providers: []
})
export class RoleManagementPageModule { }