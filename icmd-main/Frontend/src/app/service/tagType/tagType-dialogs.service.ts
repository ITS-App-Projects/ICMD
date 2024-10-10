import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { TagTypeAddEditDialogComponent } from "@p/dialog/masters/tagType/tagType-add-edit-dialog";

@Injectable()
export class TagTypeDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openTagTypeDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            TagTypeAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            TagTypeAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}