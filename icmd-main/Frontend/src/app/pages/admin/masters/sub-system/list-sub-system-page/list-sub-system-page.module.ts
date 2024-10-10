import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListSubSystemPageComponent } from "./list-sub-system-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListSubSystemPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListSubSystemPageModule { }
