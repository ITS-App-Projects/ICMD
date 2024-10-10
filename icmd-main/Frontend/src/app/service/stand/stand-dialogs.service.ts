import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { StandAddEditDialogComponent } from "@p/dialog/masters/stand/stand-add-edit-dialog";

@Injectable()
export class StandDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openStandDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            StandAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            StandAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}