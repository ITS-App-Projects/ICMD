import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListBankPageComponent } from "./list-bank-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListBankPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListBankPageModule { }
