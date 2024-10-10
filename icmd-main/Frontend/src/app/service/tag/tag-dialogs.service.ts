import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel, ProjectTagFieldInfoDtoModel } from "@m/common";
import { TagAddEditDialogComponent, TagDialogInputDataModel } from "@p/dialog/masters/tag/tag-add-edit-dialog";

@Injectable()
export class TagDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openTagDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            TagAddEditDialogComponent,
            TagDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            TagAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 400
        );
    }
}