import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { MenuAddEditDialogComponent, MenuDialogInputDataModel, MenuDialogOutputDataModel } from "@p/dialog/menu/menu-add-edit-dialog";

@Injectable()
export class MenuDialogsService {
  constructor(private _dialogs: DialogsService) {}

  public async openMenuDialog(
    menuId: string,
    sortOrder: number,
    isSubMenu: boolean = false,
    mainMenuId: string = "00000000-0000-0000-0000-000000000000"
  ): Promise<CreateOrEditMenuModel> {
    return this._dialogs.openDialog<
      MenuAddEditDialogComponent,
      MenuDialogInputDataModel,
      MenuDialogOutputDataModel,
      CreateOrEditMenuModel
    >(
      MenuAddEditDialogComponent,
      { menuId, sortOrder, isSubMenu, mainMenuId },
      (model) => (model.success ? model.menu : null),
    );
  }
}
