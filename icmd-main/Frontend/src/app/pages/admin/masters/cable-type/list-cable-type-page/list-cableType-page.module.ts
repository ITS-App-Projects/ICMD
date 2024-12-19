import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListCableTypePageComponent } from "./list-cableType-page.component"; 
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListCableTypePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListCableTypePageModule { }
