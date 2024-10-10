import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListSystemPageComponent } from "./list-system-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListSystemPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListSystemPageModule { }
