import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListInstrumentPageComponent } from ".";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListInstrumentPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListInstrumentPageModule { }