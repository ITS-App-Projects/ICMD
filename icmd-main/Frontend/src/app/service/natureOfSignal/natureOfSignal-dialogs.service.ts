import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { NatureOfSignalAddEditDialogComponent } from "@p/dialog/masters/natureOfSignal/natureOfSignal-add-edit-dialog";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";

@Injectable()
export class NatureOfSignalDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openNatureOfSignalDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            NatureOfSignalAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            NatureOfSignalAddEditDialogComponent,
            { id },
            (model) => model.success, 700
        );
    }
}