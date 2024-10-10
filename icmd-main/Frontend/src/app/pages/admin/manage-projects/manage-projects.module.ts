import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ManageProjectsRoutingModule } from "./manage-projects-routing.module";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, ManageProjectsRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
    declarations: [],
})
export class ManageProjectsModule { }