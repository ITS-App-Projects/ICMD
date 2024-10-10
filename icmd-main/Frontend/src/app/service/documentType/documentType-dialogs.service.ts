import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { DocumentTypeAddEditDialogComponent } from "@p/dialog/masters/documentType/document-type-add-edit-dialog";

@Injectable()
export class DocumentTypeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openDocumentTypeDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            DocumentTypeAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            DocumentTypeAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}