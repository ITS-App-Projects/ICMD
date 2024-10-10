import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { WorkAreaPackAddEditDialogComponent } from "@p/dialog/masters/work-area-pack/work-area-pack-add-edit-dialog";

@Injectable()
export class WorkAreaPackDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openWorkAreaPackDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            WorkAreaPackAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            WorkAreaPackAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 500
        );
    }
}