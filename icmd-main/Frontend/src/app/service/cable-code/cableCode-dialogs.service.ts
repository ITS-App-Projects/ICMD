import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

import { CableCodeAddEditDialogComponent } from "@p/dialog/masters/cable-code/cable-code-add-edit-dialog";


@Injectable()
export class CableCodeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openCableCodeDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        CableCodeAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            CableCodeAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}