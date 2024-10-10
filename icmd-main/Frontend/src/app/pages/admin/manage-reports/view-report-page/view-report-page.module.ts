import { Route, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { ViewReportPageComponent } from "./view-report-page.component";

const routes: Route[] = [
    {
        path: "",
        component: ViewReportPageComponent,
    },
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forChild(routes),
    ],
    providers: []
})
export class ViewReportPageModule { }