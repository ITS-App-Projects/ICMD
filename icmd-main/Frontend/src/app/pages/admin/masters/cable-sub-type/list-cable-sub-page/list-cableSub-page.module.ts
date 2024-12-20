import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListCableSubPageComponent } from "./list-cableSub-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListCableSubPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListCableSubPageModule { }
