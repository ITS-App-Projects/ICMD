import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditSubProcessDtoModel } from "@c/masters/sub-process/create-edit-sub-process-form";
import { SubProcessInfoDtoModel } from "@c/masters/sub-process/list-sub-process-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class SubProcessService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SubProcessInfoDtoModel>> {
        return this._http.post<PagedResultModel<SubProcessInfoDtoModel>>(
            `${environment.apiUrl}SubProcess/GetAllSubProcess`,
            request
        );
    }

    public getSubProcessInfo(id: string): Observable<SubProcessInfoDtoModel> {
        return this._http.get<SubProcessInfoDtoModel>(
            `${environment.apiUrl}SubProcess/GetSubProcessInfo?id=${id}`
        );
    }

    public createEditSubProcess(info: CreateOrEditSubProcessDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}SubProcess/CreateOrEditSubProcess`,
            info
        );
    }

    public deleteSubProcess(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}SubProcess/DeleteSubProcess?id=${id}`
        );
    }

    public deleteBulkSubProcess(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}SubProcess/DeleteBulkSubProcesses`, {
                body: ids
            }
        );
    }

    public validateImportSubProcess(projectId: string, file: File): Observable<ImportFileResultModel<SubProcessInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SubProcessInfoDtoModel>> (
            `${environment.apiUrl}SubProcess/ValidateImportSubProcess`,
            formData
        );
    }

    public importSubProcess(projectId: string, file: File): Observable<ImportFileResultModel<SubProcessInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<SubProcessInfoDtoModel>>(
            `${environment.apiUrl}SubProcess/ImportSubProcess`,
            formData
        );
    }
}