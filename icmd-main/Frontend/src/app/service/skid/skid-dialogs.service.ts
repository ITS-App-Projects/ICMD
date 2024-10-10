import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { SkidAddEditDialogComponent } from "@p/dialog/masters/skid/skid-add-edit-dialog";

@Injectable()
export class SkidDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openSkidDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            SkidAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            SkidAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}