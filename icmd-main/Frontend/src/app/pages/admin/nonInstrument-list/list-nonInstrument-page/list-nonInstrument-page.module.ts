import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListNonInstrumentPageComponent } from "./list-nonInstrument-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";

const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListNonInstrumentPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: [PermissionGuard]
})
export class ListNonInstrumentPageModule { }