import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { MenuInfoModel } from "@c/menu-role/menu-list-table";
import { environment } from "@env/environment";
import { BaseDataResponseModel, BaseResponseModel } from "@m/auth/login-response-model";
import { MenuListWithRoleModel, RoleMenuPermissionModel, SortOrderModel } from "@m/menu";
import { Observable } from "rxjs";

@Injectable()
export class MenuService {
  constructor(private _http: HttpClient) { }

  public getAllWithPermission(): Observable<MenuListWithRoleModel> {
    return this._http.get<MenuListWithRoleModel>(
      `${environment.apiUrl}Menu/GetAllMenuWithPermission`
    );
  }

  public saveRolePermission(
    model: RoleMenuPermissionModel[]
  ): Observable<BaseResponseModel> {
    return this._http.post<BaseResponseModel>(
      `${environment.apiUrl}Menu/SetRoleWiseMenuPermission`,
      model
    );
  }

  public getAllMenus(): Observable<MenuInfoModel[]> {
    return this._http.get<MenuInfoModel[]>(
      `${environment.apiUrl}Menu/GetAllMenu`
    );
  }

  public deleteMenu(id: string): Observable<BaseResponseModel> {
    return this._http.get<BaseResponseModel>(
      `${environment.apiUrl}Menu/DeleteMenuById?menuId=${id}`
    );
  }

  public changeSortOrderOfMenu(model: SortOrderModel[]): Observable<BaseResponseModel> {
    return this._http.post<BaseResponseModel>(
      `${environment.apiUrl}Menu/ChangeSortOrderOfMenu`,
      model
    );
  }

  public createOrEditMenu(
    menuInfo: CreateOrEditMenuModel
  ): Observable<BaseDataResponseModel<CreateOrEditMenuModel>> {
    return this._http.post<BaseDataResponseModel<CreateOrEditMenuModel>>(
      `${environment.apiUrl}Menu/CreateOrEditMenu`,
      menuInfo
    );
  }

  public getMenuById(id: string): Observable<CreateOrEditMenuModel> {
    return this._http.get<CreateOrEditMenuModel>(
      `${environment.apiUrl}Menu/GetMenuById?menuId=${id}`
    );
  }
}