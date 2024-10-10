import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListHierarchyPageComponent } from "./list-hierarchy-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListHierarchyPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListHierarchyPageModule { }