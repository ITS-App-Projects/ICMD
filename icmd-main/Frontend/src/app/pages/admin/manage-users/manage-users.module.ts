import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ManageUsersRoutingModule } from "./manage-users-routing.module";
import { ColumnSelectorDialogsModule } from "@p/dialog/column-selector/column-selector.module";

@NgModule({
    imports: [CommonModule, ManageUsersRoutingModule, ColumnSelectorDialogsModule],
    exports: [],
    providers: [],
    declarations: [],
})
export class ManageUsersModule { }