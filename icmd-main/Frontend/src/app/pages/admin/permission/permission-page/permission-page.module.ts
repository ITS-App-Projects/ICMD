import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { PermissionPageComponent } from "./permission-page.component";

const routes: Route[] = [
    {
        path: "",
        component: PermissionPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: []
})
export class PermissionManagementPageModule { }