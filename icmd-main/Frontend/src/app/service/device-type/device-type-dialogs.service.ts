import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { DeviceTypeAddEditDialogComponent } from "@p/dialog/masters/device-type/device-type-add-edit-dialog";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";

@Injectable()
export class DeviceTypeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openDeviceTypeDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            DeviceTypeAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            DeviceTypeAddEditDialogComponent,
            { id },
            (model) => model.success, 700
        );
    }
}