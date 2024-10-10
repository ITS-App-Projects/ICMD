import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListNatureOfSignalPageComponent } from "./list-natureOfSignal-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListNatureOfSignalPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListNatureOfSignalPageModule { }
