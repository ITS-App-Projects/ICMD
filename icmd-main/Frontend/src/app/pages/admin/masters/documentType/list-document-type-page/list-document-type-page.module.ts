import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListDocumentTypePageComponent } from "./list-document-type-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListDocumentTypePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListDocumentTypePageModule { }
