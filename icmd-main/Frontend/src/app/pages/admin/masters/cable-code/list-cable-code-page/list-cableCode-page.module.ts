import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListCableCodePageComponent } from "./list-cableCode-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListCableCodePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListCableCodePageModule { }
