import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListTagTypePageComponent } from "./list-tagType-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListTagTypePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListTagTypePageModule { }
