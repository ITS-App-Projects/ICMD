import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListTagPageComponent } from "./list-tag-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListTagPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListTagPageModule { }
