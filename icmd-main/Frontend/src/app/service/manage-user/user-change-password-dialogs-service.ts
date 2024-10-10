import { UserChangePasswordDialogComponent, UserChangePasswordDialogInputDataModel, UserChangePasswordDialogOutputDataModel } from "@p/dialog/user/user-change-password-dialog";
import { DialogsService } from "../common";
import { Injectable } from "@angular/core";

@Injectable()
export class UserChangePasswordDialogService {
    constructor(private _dialogs: DialogsService) { }

    public async openChangePasswordDialog(
        id: string,
    ): Promise<void> {
        return this._dialogs.openDialog<
            UserChangePasswordDialogComponent,
            UserChangePasswordDialogInputDataModel,
            UserChangePasswordDialogOutputDataModel,
            void
        >(
            UserChangePasswordDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}