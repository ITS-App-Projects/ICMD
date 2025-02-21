import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditProcessDtoModel } from "@c/masters/process/create-edit-process-form";
import { ProcessInfoDtoModel } from "@c/masters/process/list-process-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ProcessService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ProcessInfoDtoModel>> {
        return this._http.post<PagedResultModel<ProcessInfoDtoModel>>(
            `${environment.apiUrl}Process/GetAllProcess`,
            request
        );
    }

    public getProcessInfo(id: string): Observable<ProcessInfoDtoModel> {
        return this._http.get<ProcessInfoDtoModel>(
            `${environment.apiUrl}Process/GetProcessInfo?id=${id}`
        );
    }

    public createEditProcess(info: CreateOrEditProcessDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Process/CreateOrEditProcess`,
            info
        );
    }

    public deleteProcess(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Process/DeleteProcess?id=${id}`
        );
    }

    public deleteBulkProcess(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
           `${environment.apiUrl}Process/DeleteBulkProcesses`, {
                body: ids,
            }
        );
    }

    public validateImportProcess(projectId: string, file: File): Observable<ImportFileResultModel<ProcessInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ProcessInfoDtoModel>> (
            `${environment.apiUrl}Process/ValidateImportProcess`,
            formData
        );
    }

    public importProcess(projectId: string, file: File): Observable<ImportFileResultModel<ProcessInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ProcessInfoDtoModel>>(
            `${environment.apiUrl}Process/ImportProcess`,
            formData
        );
    }
}