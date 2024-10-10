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
                        "./list-project-page/list-project-page.module"
                    ).then((m) => m.ListProjectPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class ManageProjectsRoutingModule { }
