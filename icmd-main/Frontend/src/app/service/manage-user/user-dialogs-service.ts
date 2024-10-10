import { Injectable } from "@angular/core";
import { UserAddEditDialogComponent, UserDialogInputDataModel, UserDialogOutputDataModel } from "@p/dialog/user/user-add-edit-dialog";
import { DialogsService } from "../common/dialogs.service";
import { RoleDetailsModel } from "@m/common";

@Injectable()
export class UserDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openUserDialog(
        id: string,
        roles: RoleDetailsModel[]
    ): Promise<void> {
        return this._dialogs.openDialog<
            UserAddEditDialogComponent,
            UserDialogInputDataModel,
            UserDialogOutputDataModel,
            void
        >(
            UserAddEditDialogComponent,
            { id, roles },
            (model) => model.success, 500
        );
    }
}