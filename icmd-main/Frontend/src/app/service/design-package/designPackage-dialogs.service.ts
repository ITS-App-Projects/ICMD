import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

import { DesignPackageAddEditDialogComponent } from "@p/dialog/masters/design-package/design-package-add-edit-dialog";


@Injectable()
export class DesignPackageDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openDesignPackageDialog(
        id: string, projectId: string
    ): Promise<void> {
        return this._dialogs.openDialog<
        DesignPackageAddEditDialogComponent,
            CommonDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            DesignPackageAddEditDialogComponent,
            { id, projectId },
            (model) => model.success, 600
        );
    }
}