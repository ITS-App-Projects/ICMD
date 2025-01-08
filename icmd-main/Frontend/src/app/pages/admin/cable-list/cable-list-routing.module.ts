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
                        "./list-cableList-page/list-cableList-page.module"
                    ).then((m) => m.ListCableListPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class CableListRoutingModule { }