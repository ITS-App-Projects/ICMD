import { NgModule } from "@angular/core";
import { ColumnSelectorDialogComponent } from "./column-selector-dialog";
import { ColumnTemplateDialogComponent } from "./add-column-template-dialog/add-column-template-dialog.component";
import { MatDialogModule } from "@angular/material/dialog";
import { ColumnTemplateService } from "src/app/service/column-selector";

@NgModule({
    imports: [MatDialogModule, ColumnSelectorDialogComponent, ColumnTemplateDialogComponent],
    exports: [],
    declarations: [],
    providers: [ColumnTemplateService],
})
export class ColumnSelectorDialogsModule { }