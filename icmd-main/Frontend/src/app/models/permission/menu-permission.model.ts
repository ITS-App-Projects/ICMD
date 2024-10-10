export interface MenuPermissionListModel {
    id: number;
    menuId: number;
    menuName: string | null;
    parentMenuName: string | null;
    operationList: OperationListModel[] | null;
  }
  
  export interface OperationListModel {
    operationId: number;
    operationName: string;
    isGranted: boolean;
    menuPermissionId: number | null;
  }
  