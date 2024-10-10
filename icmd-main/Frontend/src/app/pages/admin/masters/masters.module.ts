import { CommonModule } from "@angular/common";
import { MastersRoutingModule } from "./masters-routing.module";
import { NgModule } from "@angular/core";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, MastersRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
})
export class MastersModule { }