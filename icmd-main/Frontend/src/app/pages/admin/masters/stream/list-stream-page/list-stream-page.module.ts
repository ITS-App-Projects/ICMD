import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListStreamPageComponent } from "./list-stream-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListStreamPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListStreamPageModule { }
