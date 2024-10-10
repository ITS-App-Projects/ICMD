import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { ProcessAddEditDialogComponent } from "@p/dialog/masters/process/process-add-edit-dialog/process-add-edit-dialog.component";

@Injectable()
export class ProcessDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openProcessDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            ProcessAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            ProcessAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}