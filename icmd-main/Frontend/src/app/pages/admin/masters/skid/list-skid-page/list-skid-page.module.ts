import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListSkidPageComponent } from "./list-skid-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListSkidPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListSkidPageModule { }
