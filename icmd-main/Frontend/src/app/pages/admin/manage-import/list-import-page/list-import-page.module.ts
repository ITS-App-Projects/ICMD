import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListImportPageComponent } from "./list-import-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListImportPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListImportPageModule {
}