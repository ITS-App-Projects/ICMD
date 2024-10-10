import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListDeviceModelPageComponent } from "./list-device-model-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListDeviceModelPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListDeviceModelPageModule { }
