import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditSubSystemDtoModel } from "@c/masters/sub-system/create-edit-sub-system-form";
import { SubSystemInfoDtoModel } from "@c/masters/sub-system/list-sub-system-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class SubSystemService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SubSystemInfoDtoModel>> {
        return this._http.post<PagedResultModel<SubSystemInfoDtoModel>>(
            `${environment.apiUrl}SubSystem/GetAllSubSystems`,
            request
        );
    }

    public getSubSystemInfo(id: string): Observable<SubSystemInfoDtoModel> {
        return this._http.get<SubSystemInfoDtoModel>(
            `${environment.apiUrl}SubSystem/GetSubSystemInfo?id=${id}`
        );
    }

    public createEditSubSystem(info: CreateOrEditSubSystemDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}SubSystem/CreateOrEditSubSystem`,
            info
        );
    }

    public deleteSubSystem(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}SubSystem/DeleteSubSystem?id=${id}`
        );
    }

    public deleteBulkSubSystem(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
           `${environment.apiUrl}SubSystem/DeleteBulkSubSystems`, {
                body: ids,
            }
        );
    }

    public getAllSubSystemInfo(systemId: string = null): Observable<SubSystemInfoDtoModel[]> {
        return this._http.get<SubSystemInfoDtoModel[]>(
            `${environment.apiUrl}SubSystem/GetAllSubSystemInfo?systemId=${systemId}`
        );
    }

    public validateImportSubSystem(projectId: string, file: File): Observable<ImportFileResultModel<SubSystemInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SubSystemInfoDtoModel>> (
            `${environment.apiUrl}SubSystem/ValidateImportSubSystem`,
            formData
        );
    }

    public importSubSystem(projectId: string, file: File): Observable<ImportFileResultModel<SubSystemInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SubSystemInfoDtoModel>>(
            `${environment.apiUrl}SubSystem/ImportSubSystem`,
            formData
        );
    }
}