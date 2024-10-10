import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListPanelPageComponent } from "./list-panel-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListPanelPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListPanelPageModule { }
