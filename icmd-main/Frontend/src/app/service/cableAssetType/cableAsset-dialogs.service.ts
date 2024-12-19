import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { CableAssetAddEditDialogComponent } from "@p/dialog/masters/cable-asset-type/cable-asset-add-edit-dialog";

@Injectable()
export class CableAssetDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openCableAssetDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        CableAssetAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            CableAssetAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}