import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { FailStateAddEditDialogComponent } from "@p/dialog/masters/failState/failState-add-edit-dialog";

@Injectable()
export class FailStateDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openFailStateDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            FailStateAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            FailStateAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}