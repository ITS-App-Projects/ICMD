import { RoleDetailsModel } from "@m/common";


export interface MenuListWithRoleModel {
    roleList: RoleDetailsModel[];
    menuList: MainMenuModel[];
}

export interface MenuModel {
    id: number;
    menuDescription: string;
    sortOrder: number;
    roleIds: string[];
}

export interface MainMenuModel extends MenuModel {
    subMenuList: SubMenuModel[];
}

export interface SubMenuModel extends MenuModel {
    parentMenuId: number;
}