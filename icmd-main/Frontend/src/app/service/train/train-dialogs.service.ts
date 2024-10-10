import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { TrainAddEditDialogComponent } from "@p/dialog/masters/train/train-add-edit-dialog";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

@Injectable()
export class TrainDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openTrainDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            TrainAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            TrainAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}