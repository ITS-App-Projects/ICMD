import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListManufacturerPageComponent } from "./list-manufacturer-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListManufacturerPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListManufacturerPageModule { }
