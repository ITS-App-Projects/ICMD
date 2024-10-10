import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListTagFieldsPageComponent } from "./list-tag-Fields-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListTagFieldsPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListTagFieldsPageModule { }
