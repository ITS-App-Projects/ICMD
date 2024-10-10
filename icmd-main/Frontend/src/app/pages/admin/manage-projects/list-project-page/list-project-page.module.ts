import { Route, RouterModule } from "@angular/router";
import { ListProjectPageComponent } from "./list-project-page.component";
import { NgModule } from "@angular/core";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListProjectPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListProjectPageModule { }