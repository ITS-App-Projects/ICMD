import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { CableListRoutingModule } from "./cable-list-routing.module";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, CableListRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
    declarations: [],
})
export class CableListModule { }