import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListCableListPageComponent } from "./list-cableList-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListCableListPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListCableListPageModule { }