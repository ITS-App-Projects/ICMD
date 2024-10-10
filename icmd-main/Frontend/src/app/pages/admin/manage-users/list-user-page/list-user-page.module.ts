import { Route, RouterModule } from "@angular/router";
import { ListUserPageComponent } from "./list-user-page.component";
import { NgModule } from "@angular/core";
import { PermissionGuard } from "src/app/guards/permission.guard";
import { OperationEnum } from "@e/common";

const userRoutes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListUserPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(userRoutes),
    ],
    providers: [PermissionGuard]
})
export class ListUserPageModule { }