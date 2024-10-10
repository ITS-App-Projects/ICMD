import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListSubProcessPageComponent } from "./list-sub-process-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListSubProcessPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListSubProcessPageModule { }
