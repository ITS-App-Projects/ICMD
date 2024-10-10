import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListEquipmentCodePageComponent } from "./list-equipmentCode-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListEquipmentCodePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListEquipmentCodePageModule { }
