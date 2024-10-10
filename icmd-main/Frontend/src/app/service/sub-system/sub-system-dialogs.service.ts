import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel } from "@m/common";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { SubSystemAddEditDialogComponent, SubSystemDialogInputDataModel } from "@p/dialog/masters/sub-system/sub-system-add-edit-dialog";

@Injectable()
export class SubSystemDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openSubSystemDialog(
        id: string, systems: SystemInfoDtoModel[]
    ): Promise<void> {
        return this._dialogs.openDialog<
            SubSystemAddEditDialogComponent,
            SubSystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            SubSystemAddEditDialogComponent,
            { id, systems },
            (model) => model.success, 500
        );
    }
}