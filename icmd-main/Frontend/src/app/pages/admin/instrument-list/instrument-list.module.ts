import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { InstrumentListRoutingModule } from "./instrument-list-routing.module";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, InstrumentListRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
    declarations: [],
})
export class InstrumentListModule { }