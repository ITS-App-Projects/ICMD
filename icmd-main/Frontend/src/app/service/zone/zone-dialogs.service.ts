import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { ZoneAddEditDialogComponent } from "@p/dialog/masters/zone/zone-add-edit-dialog";

@Injectable()
export class ZoneDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openZoneDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            ZoneAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            ZoneAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}