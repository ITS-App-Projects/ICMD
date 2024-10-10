import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonDialogOutputDataModel } from "@m/common";
import { SystemAddEditDialogComponent, SystemDialogInputDataModel } from "@p/dialog/masters/system/system-add-edit-dialog";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";

@Injectable()
export class SystemDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openSystemDialog(
        id: string, workAreaPacks: WorkAreaPackInfoDtoModel[]
    ): Promise<void> {
        return this._dialogs.openDialog<
            SystemAddEditDialogComponent,
            SystemDialogInputDataModel,
            CommonDialogOutputDataModel,
            void
        >(
            SystemAddEditDialogComponent,
            { id, workAreaPacks },
            (model) => model.success, 500
        );
    }
}