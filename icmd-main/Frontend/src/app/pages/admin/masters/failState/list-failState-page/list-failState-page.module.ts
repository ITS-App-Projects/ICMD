import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListFailStatePageComponent } from "./list-failState-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListFailStatePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListFailStatePageModule { }
