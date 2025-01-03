import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

import { RoomAddEditDialogComponent } from "@p/dialog/masters/room/room-add-edit-dialog";

@Injectable()
export class RoomDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openRoomDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        RoomAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            RoomAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}