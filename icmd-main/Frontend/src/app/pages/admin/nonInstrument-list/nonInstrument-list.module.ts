import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { NonInstrumentListRoutingModule } from "./nonInstrument-list-routing.module";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, NonInstrumentListRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
    declarations: [],
})
export class NonInstrumentListModule { }