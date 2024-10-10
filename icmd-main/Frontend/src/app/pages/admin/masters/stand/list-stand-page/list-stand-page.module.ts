import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListStandPageComponent } from "./list-stand-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListStandPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListStandPageModule { }
