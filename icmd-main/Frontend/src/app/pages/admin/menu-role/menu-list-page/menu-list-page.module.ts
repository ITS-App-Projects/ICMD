import { Route, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { MenuListPageComponent } from "./menu-list-page.component";

const userRoutes: Route[] = [
    {
        path: "",
        component: MenuListPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(userRoutes),
    ],
    providers: []
})
export class MenuListPageModule { }