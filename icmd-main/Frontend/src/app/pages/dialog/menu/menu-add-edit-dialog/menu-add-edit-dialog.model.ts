import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";


export interface MenuDialogInputDataModel {
    menuId: string;
    sortOrder: number;
    isSubMenu: boolean;
    mainMenuId: string;
}

export interface MenuDialogOutputDataModel {
    success: boolean;
    menu: CreateOrEditMenuModel;
}
