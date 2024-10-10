import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { SubProcessAddEditDialogComponent } from "@p/dialog/masters/sub-process/sub-process-add-edit-dialog";

@Injectable()
export class SubProcessDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openSubProcessDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            SubProcessAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            SubProcessAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}