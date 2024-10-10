import { Injectable } from "@angular/core";
import { TemplateListModel, ColumnTemplateDialogInputDataModel, ColumnTemplateDialogOutputDataModel } from "@p/dialog/column-selector/add-column-template-dialog";
import { ColumnTemplateDialogComponent } from "@p/dialog/column-selector/add-column-template-dialog/add-column-template-dialog.component";
import { DialogsService } from "../common";

@Injectable()
export class ColumnTemplateDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openColumnTemplateDialog(templateList: TemplateListModel[], currentSelectedColumns: string) {
        return this._dialogs.openDialog<
            ColumnTemplateDialogComponent,
            ColumnTemplateDialogInputDataModel,
            ColumnTemplateDialogOutputDataModel,
            ColumnTemplateDialogOutputDataModel
        >(
            ColumnTemplateDialogComponent,
            { userDefinedTemplates: templateList, currentSelectedColumns },
            (model) => model, 500
        );
    }
}