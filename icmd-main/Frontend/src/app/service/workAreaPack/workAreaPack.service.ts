import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditWorkAreaPackDtoModel } from "@c/masters/workAreaPack/create-edit-work-area-form";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class WorkAreaPackService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<WorkAreaPackInfoDtoModel>> {
        return this._http.post<PagedResultModel<WorkAreaPackInfoDtoModel>>(
            `${environment.apiUrl}WorkAreaPack/GetAllWorkAreaPacks`,
            request
        );
    }

    public getWorkAreaPackInfo(id: string): Observable<WorkAreaPackInfoDtoModel> {
        return this._http.get<WorkAreaPackInfoDtoModel>(
            `${environment.apiUrl}WorkAreaPack/GetWorkAreaPackInfo?id=${id}`
        );
    }

    public createEditWorkAreaPack(info: CreateOrEditWorkAreaPackDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}WorkAreaPack/CreateOrEditWorkAreaPack`,
            info
        );
    }

    public deleteWorkAreaPack(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}WorkAreaPack/DeleteWorkAreaPack?id=${id}`
        );
    }

    public deleteBulkWorkAreaPack(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}WorkAreaPack/DeleteBulkWorkAreaPacks`, {
                body: ids,
            }
        );
    }

    public getAllWorkAreaPackInfo(projectId: string): Observable<WorkAreaPackInfoDtoModel[]> {
        return this._http.get<WorkAreaPackInfoDtoModel[]>(
            `${environment.apiUrl}WorkAreaPack/GetAllWorkAreaPackInfo?projectId=${projectId}`
        );
    }

    public validateImportWap(projectId: string, file: File): Observable<ImportFileResultModel<WorkAreaPackInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<WorkAreaPackInfoDtoModel>> (
            `${environment.apiUrl}WorkAreaPack/ValidateWorkAreaPack`,
            formData
        );
    }

    public importWorkAreaPack(projectId: string, file: File): Observable<ImportFileResultModel<WorkAreaPackInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<WorkAreaPackInfoDtoModel>>(
            `${environment.apiUrl}WorkAreaPack/ImportWorkAreaPack`,
            formData
        );
    }
}