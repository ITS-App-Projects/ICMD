import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListDesignPackagePageComponent } from "./list-designPackage-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListDesignPackagePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListDesignPackagePageModule { }
