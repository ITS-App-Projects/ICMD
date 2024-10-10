export interface MenuInfoModel {
    id: string;
    menuName: string;
    menuDescription: string;
    controllerName: string;
    icon: string;
    url: string;
    sortOrder: number;
    parentMenuId: number | null;
    isActive: boolean;
    subMenus: MenuInfoModel[];
  }