import { Injectable } from "@angular/core";
import { DialogsService } from "../common";
import { MenuInfoModel } from "@c/menu-role/menu-list-table";
import { ListSubMenuDialogComponent, ListSubMenuDialogInputDataModel, ListSubMenuDialogOutputDataModel } from "@p/dialog/menu/list-submenu-dialog";

@Injectable()
export class SubMenuDialogsService {
  constructor(private _dialogs: DialogsService) {}

  public async openSubMenuListDialog(
    subMenu: MenuInfoModel[],
    menuId: string
  ): Promise<void> {
    return this._dialogs.openDialog<
      ListSubMenuDialogComponent,
      ListSubMenuDialogInputDataModel,
      ListSubMenuDialogOutputDataModel,
      void
    >(
      ListSubMenuDialogComponent,
      { subMenus: subMenu, menuId: menuId },
      (model) => model.success,
      1000
    );
  }
}
