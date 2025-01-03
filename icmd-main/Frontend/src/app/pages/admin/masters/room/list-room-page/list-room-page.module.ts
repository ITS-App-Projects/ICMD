import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListRoomPageComponent } from "./list-room-page.component";
import { PermissionGuard } from "src/app/guards/permission.guard";


const routes: Route[] = [
    {
        path: "",
        canActivate: [PermissionGuard],
        component: ListRoomPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forChild(routes)],
    providers: [PermissionGuard]
})
export class ListRoomPageModule { }
