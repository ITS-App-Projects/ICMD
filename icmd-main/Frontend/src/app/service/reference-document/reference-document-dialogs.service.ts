import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, DropdownInfoDtoModel } from "@m/common";
import { ReferenceDocumentDialogInputDataModel } from "@p/dialog/masters/reference-document/reference-document-add-edit-dialog/reference-document-add-edit-dialog.model";
import { ReferenceDocumentAddEditDialogComponent } from "@p/dialog/masters/reference-document/reference-document-add-edit-dialog/reference-document-add-edit-dialog.component";

@Injectable()
export class ReferenceDocumentDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openReferenceDocumentDialog(
        id: string, projectId: string, documentType: DropdownInfoDtoModel[]
    ): Promise<void> {
        return this._dialogs.openDialog<
            ReferenceDocumentAddEditDialogComponent,
            ReferenceDocumentDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            ReferenceDocumentAddEditDialogComponent,
            { id, projectId, documentType },
            (model) => model.success, 700
        );
    }
}