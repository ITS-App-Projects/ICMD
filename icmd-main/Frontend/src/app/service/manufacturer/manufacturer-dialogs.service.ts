import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { ManufacturerAddEditDialogComponent } from "@p/dialog/masters/manufacturer/manufacturer-add-edit-dialog";

@Injectable()
export class ManufacturerDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openManufacturerDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            ManufacturerAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            ManufacturerAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}