import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { PermissionGuard } from "src/app/guards/permission.guard";
import { UserProfilePageComponent } from ".";

const userRoutes: Route[] = [
    {
        path: "",
        // canActivate: [PermissionGuard],
        component: UserProfilePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(userRoutes),
    ],
    providers: [PermissionGuard]
})
export class UserProfilePageModule { }
  