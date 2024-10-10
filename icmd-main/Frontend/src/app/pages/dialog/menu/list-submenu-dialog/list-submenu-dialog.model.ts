import { MenuInfoModel } from "@c/menu-role/menu-list-table";


export interface ListSubMenuDialogInputDataModel {
  subMenus: MenuInfoModel[];
  menuId: string;
}

export interface ListSubMenuDialogOutputDataModel {
  success: boolean;
}
