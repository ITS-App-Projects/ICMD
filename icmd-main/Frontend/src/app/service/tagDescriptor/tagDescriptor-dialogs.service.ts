import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { TagDescriptorAddEditDialogComponent } from "@p/dialog/masters/tagDescriptor/tagDescriptor-add-edit-dialog";

@Injectable()
export class TagDescriptorDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openTagDescriptorDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            TagDescriptorAddEditDialogComponent,
            CommonSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            TagDescriptorAddEditDialogComponent,
            { id },
            (model) => model.success, 500
        );
    }
}