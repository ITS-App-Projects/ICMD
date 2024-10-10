import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListJunctionBoxPageComponent } from "./list-junction-box-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListJunctionBoxPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListJunctionBoxPageModule { }
