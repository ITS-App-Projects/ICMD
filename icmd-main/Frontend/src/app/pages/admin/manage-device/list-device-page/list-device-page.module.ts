import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ListDevicePageComponent } from "./list-device-page.component";

const routes: Route[] = [
    {
        path: "",
        component: ListDevicePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: []
})
export class ListDevicePageModule { }