import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { StreamAddEditDialogComponent } from "@p/dialog/masters/stream/stream-add-edit-dialog";

@Injectable()
export class StreamDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openStreamDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            StreamAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            StreamAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}