import { NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import { ViewDevicePageComponent } from "./view-device-page.component";

const routes: Route[] = [
    {
        path: "",
        component: ViewDevicePageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: []
})
export class ViewDevicePageModule { }