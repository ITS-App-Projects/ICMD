import { Route, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { ListLogsPageComponent } from "./list-logs-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListLogsPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListLogsPageModule { }