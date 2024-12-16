import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListCableAssetPageComponent } from "./list-cableAsset-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListCableAssetPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListCableAssetPageModule { }
