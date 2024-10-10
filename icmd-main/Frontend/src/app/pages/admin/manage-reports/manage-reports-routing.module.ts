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
                        "./list-reports-page/list-reports-page.module"
                    ).then((m) => m.ListReportsPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class ManageReportsRoutingModule { }
