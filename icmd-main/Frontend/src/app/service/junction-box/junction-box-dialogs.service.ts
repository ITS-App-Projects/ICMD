import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { JunctionBoxAddEditDialogComponent } from "@p/dialog/masters/junction-box/junction-box-add-edit-dialog";

@Injectable()
export class JunctionBoxDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openJunctionBoxDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            JunctionBoxAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            JunctionBoxAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}