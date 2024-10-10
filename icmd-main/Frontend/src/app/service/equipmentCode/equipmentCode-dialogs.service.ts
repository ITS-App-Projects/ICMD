import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { EquipmentCodeAddEditDialogComponent } from "@p/dialog/masters/equipmentCode/equipmentCode-add-edit-dialog";

@Injectable()
export class EquipmentCodeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openEquipmentCodeDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            EquipmentCodeAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            EquipmentCodeAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}