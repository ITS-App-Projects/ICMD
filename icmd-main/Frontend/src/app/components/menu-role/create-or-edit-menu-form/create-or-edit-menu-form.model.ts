export interface CreateOrEditMenuModel {
    id: string;
    menuName: string;
    menuDescription: string;
    controllerName: string;
    icon: string;
    url: string;
    sortOrder: number;
    parentMenuId: number | null;
    isPermission: boolean;
  }
  