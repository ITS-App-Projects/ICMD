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
                        "./list-hierarchy-page/list-hierarchy-page.module"
                    ).then((m) => m.ListHierarchyPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class HierarchyRoutingModule { }