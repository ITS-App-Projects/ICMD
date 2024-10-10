import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListProcessPageComponent } from "./list-process-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListProcessPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListProcessPageModule { }
