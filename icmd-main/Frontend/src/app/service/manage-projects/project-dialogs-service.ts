import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { ProjectAddEditDialogComponent, ProjectDialogInputDataModel, ProjectDialogOutputDataModel } from "@p/dialog/projects/project-add-edit-dialog";

@Injectable()
export class ProjectDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openProjectDialog(
        id: string
    ): Promise<void> {
        return this._dialogs.openDialog<
            ProjectAddEditDialogComponent,
            ProjectDialogInputDataModel,
            ProjectDialogOutputDataModel,
            void
        >(
            ProjectAddEditDialogComponent,
            { id },
            (model) => model.success, 700
        );
    }
}