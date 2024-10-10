import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CommonColumnSelectorDialogInputDataModel, CommonColumnSelectorDialogOutputDataModel } from "@m/common";
import { ColumnSelectorDialogComponent } from "@p/dialog/column-selector/column-selector-dialog";

@Injectable()
export class ColumnSelectorDialogsService {
    constructor(private _dialogs: DialogsService) { }

    public async openColumnSelectorDialog(
        columnList: {
            key: string;
            label: string;
        }[],
        listName: string
    ): Promise<CommonColumnSelectorDialogOutputDataModel> {
        return this._dialogs.openDialog<
            ColumnSelectorDialogComponent,
            CommonColumnSelectorDialogInputDataModel,
            CommonColumnSelectorDialogOutputDataModel,
            CommonColumnSelectorDialogOutputDataModel
        >(
            ColumnSelectorDialogComponent,
            { itemList: columnList, listName: listName },
            (model) => model, 900
        );
    }
}