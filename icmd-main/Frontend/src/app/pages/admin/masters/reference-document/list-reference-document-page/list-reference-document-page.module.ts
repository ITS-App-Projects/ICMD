import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListReferenceDocumentPageComponent } from "./list-reference-document-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListReferenceDocumentPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListReferenceDocumentPageModule { }
