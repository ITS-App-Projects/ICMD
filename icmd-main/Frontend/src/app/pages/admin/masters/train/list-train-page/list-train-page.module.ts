import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListTrainPageComponent } from "./list-train-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListTrainPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListTrainPageModule { }
