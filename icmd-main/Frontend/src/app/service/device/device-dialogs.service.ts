import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { DeviceViewDialogComponent } from "@p/dialog/device/device-view-dialog";

@Injectable()
export class DeviceDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openDeviceViewDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            DeviceViewDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            DeviceViewDialogComponent,
            { id },
            (model) => model.success, 900
        );
    }
}