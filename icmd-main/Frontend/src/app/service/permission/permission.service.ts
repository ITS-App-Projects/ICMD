import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import {
  MenuPermissionListModel,
  PermissionByMenuRoleModel,
} from "@m/permission";
import { Observable } from "rxjs";

@Injectable()
export class PermissionService {
  constructor(private _http: HttpClient) {}

  public GetAllMenuWithPermission(
    roleId: string
  ): Observable<MenuPermissionListModel[]> {
    return this._http.get<MenuPermissionListModel[]>(
      `${environment.apiUrl}Permission/GetAllMenuWithPermission?roleId=${roleId}`
    );
  }

  public setPermissionByRole(
    model: PermissionByMenuRoleModel[]
  ): Observable<BaseResponseModel> {
    return this._http.post<BaseResponseModel>(
      `${environment.apiUrl}Permission/SetPermissionByRole`,
      model
    );
  }
}
