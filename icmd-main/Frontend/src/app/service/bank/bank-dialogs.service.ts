import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { BankAddEditDialogComponent } from "@p/dialog/masters/bank/bank-add-edit-dialog"
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

@Injectable()
export class BankDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openBankDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            BankAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            BankAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}