import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditSystemDtoModel } from "@c/masters/system/create-edit-system-form";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class SystemService {
    constructor(private _http: HttpClient) { }
    private emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SystemInfoDtoModel>> {
        return this._http.post<PagedResultModel<SystemInfoDtoModel>>(
            `${environment.apiUrl}System/GetAllSystems`,
            request
        );
    }

    public getSystemInfo(id: string): Observable<SystemInfoDtoModel> {
        return this._http.get<SystemInfoDtoModel>(
            `${environment.apiUrl}System/GetSystemInfo?id=${id}`
        );
    }

    public createEditSystem(info: CreateOrEditSystemDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}System/CreateOrEditSystem`,
            info
        );
    }

    public deleteSystem(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}System/DeleteSystem?id=${id}`
        );
    }

    public deleteBulkSystem(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
           `${environment.apiUrl}System/DeleteBulkSystems`, {
                body: ids,
            }
        );
    }

    public getAllSystemInfo(projectId: string, workAreaPackId: string = this.emptyGuid): Observable<SystemInfoDtoModel[]> {
        return this._http.get<SystemInfoDtoModel[]>(
            `${environment.apiUrl}System/GetAllSystemInfo?projectId=${projectId}&workAreaPackId=${workAreaPackId}`
        );
    }

    public validateImportSystem(projectId: string, file: File): Observable<ImportFileResultModel<SystemInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SystemInfoDtoModel>> (
            `${environment.apiUrl}System/ValidateImportSystem`,
            formData
        );
    }

    public importSystem(projectId: string, file: File): Observable<ImportFileResultModel<SystemInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SystemInfoDtoModel>>(
            `${environment.apiUrl}System/ImportSystem`,
            formData
        );
    }
}