import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

import { CableTypeAddEditDialogComponent } from "@p/dialog/masters/cable-type/cable-type-add-edit-dialog";

@Injectable()
export class CableTypeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openCableTypeDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        CableTypeAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            CableTypeAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}