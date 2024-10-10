import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListTagDescriptorPageComponent } from "./list-tagDescriptor-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListTagDescriptorPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListTagDescriptorPageModule { }
