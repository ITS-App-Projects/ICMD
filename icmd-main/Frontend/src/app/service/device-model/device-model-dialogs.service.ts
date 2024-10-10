import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, DropdownInfoDtoModel } from "@m/common";
import { DeviceModelAddEditDialogComponent, DeviceModelDialogInputDataModel } from "@p/dialog/masters/device-model/device-model-add-edit-dialog";

@Injectable()
export class DeviceModelDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openDeviceModelDialog(
        id: string, manufacturers: DropdownInfoDtoModel[]
    ): Promise<void> {
        return this._dialogs.openDialog<
            DeviceModelAddEditDialogComponent,
            DeviceModelDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            DeviceModelAddEditDialogComponent,
            { id, manufacturers },
            (model) => model.success, 700
        );
    }
}