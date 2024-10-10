import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ChangeUserPasswordModel } from "@c/manange-user/change-user-password-form";
import { CreateOrEditUserModel, UpdateUserModel, ViewUserDetails } from "@c/manange-user/create-edit-user-form";
import { ListUserTableModel } from "@c/manange-user/list-user-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { PagedAndSortedResultRequestModel, PagedResultModel, RoleDetailsModel, UserDetailsModel } from "@m/common";
import { ChangePasswordModel } from "@p/admin/manage-users/user-change-password";
import { Observable } from "rxjs";

@Injectable()
export class UserService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ListUserTableModel>> {
        return this._http.post<PagedResultModel<ListUserTableModel>>(
            `${environment.apiUrl}User/GetAllUser`,
            request
        );
    }

    public getUserInfo(id: string): Observable<ViewUserDetails> {
        return this._http.get<ViewUserDetails>(
            `${environment.apiUrl}User/GetUserInfo?userId=${id}`
        );
    }

    public createUser(userInfo: CreateOrEditUserModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}User/CreateUser`,
            userInfo
        );
    }

    public updateUser(
        userInfo: UpdateUserModel
    ): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}User/UpdateUser`,
            userInfo
        );
    }

    public updateMyProfile(
        userInfo: UpdateUserModel
    ): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}User/UpdateMyProfile`,
            userInfo
        );
    }

    public deleteUser(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}User/DeleteUser?userId=${id}`
        );
    }

    public getallRoles(): Observable<RoleDetailsModel[]> {
        return this._http.get<RoleDetailsModel[]>(
            `${environment.apiUrl}User/GetAllRolesInfo`
        );
    }

    public getAllUsersInfo(): Observable<UserDetailsModel[]> {
        return this._http.get<UserDetailsModel[]>(
            `${environment.apiUrl}User/GetAllUserInfo`
        );
    }

    public updateUserProject(projectId: string): Observable<boolean> {
        return this._http.get<boolean>(
            `${environment.apiUrl}User/UpdateUserProject?projectId=${projectId}`
        );
    }

    public updateCurrentUserPassword(
        info: ChangePasswordModel
    ): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Account/ChangePassword`,
            info
        );
    }

    public updateUserPassword(
        info: ChangeUserPasswordModel
    ): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Account/ChangeUserPassword`,
            info
        );
    }
}