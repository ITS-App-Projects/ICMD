import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListDeviceTypePageComponent } from "./list-device-type-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListDeviceTypePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListDeviceTypePageModule { }
