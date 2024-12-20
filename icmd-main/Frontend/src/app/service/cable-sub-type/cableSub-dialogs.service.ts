import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

import { CableSubAddEditDialogComponent } from "@p/dialog/masters/cable-sub-type/cable-sub-add-edit-dialog";


@Injectable()
export class CableSubDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openCableSubDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        CableSubAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            CableSubAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}