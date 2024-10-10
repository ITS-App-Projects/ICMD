export interface MenuItemModel {
    id: string;
    menuDescription: string;
    icon: string;
    url: string;
    sortOrder: number;
    isPermission: boolean;
    //permissions: string[];
  }
  
  export interface MenuItemListModel extends MenuItemModel {
    subMenu: MenuItemModel[];
  }
  
  export interface UserPermissionModel {
    menuName: string;
    url: string;
    controllerName: string;
    permissionName: string[];
  }
  
  export interface MenuAndPermissionListDto {
    permissions: UserPermissionModel[];
    menuItems: MenuItemListModel[];
  }
  