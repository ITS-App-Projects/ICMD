import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { PanelAddEditDialogComponent } from "@p/dialog/masters/panel/panel-add-edit-dialog";

@Injectable()
export class PanelDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openPanelDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            PanelAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            PanelAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}