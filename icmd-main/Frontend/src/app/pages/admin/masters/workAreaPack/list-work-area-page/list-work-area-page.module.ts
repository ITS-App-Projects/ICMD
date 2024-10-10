import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListWorkAreaPageComponent } from "./list-work-area-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListWorkAreaPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListWorkAreaPageModule { }
