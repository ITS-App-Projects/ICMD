import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

const routes: Routes = [
    {
        path: "",
        canActivate: [],
        children: [
            {
                path: "",
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./list-logs-page/list-logs-page.module"
                    ).then((m) => m.ListLogsPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class ManageLogsRoutingModule { }
