import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditJunctionBoxDtoModel } from "@c/masters/junction-box/create-edit-junction-box-form";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ActiveInActiveDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class SkidService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._http.post<PagedResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}Skid/GetAllSkids`,
            request
        );
    }

    public getSkidInfo(id: string): Observable<CreateOrEditJunctionBoxDtoModel> {
        return this._http.get<CreateOrEditJunctionBoxDtoModel>(
            `${environment.apiUrl}Skid/GetSkidInfo?id=${id}`
        );
    }

    public createEditSkid(info: CreateOrEditJunctionBoxDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Skid/CreateOrEditSkid`,
            info
        );
    }

    public deleteSkid(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Skid/DeleteSkid?id=${id}`
        );
    }

    public deleteBulkSkid(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Skid/DeleteBulkSkids`, {
                body: ids
            }
        );
    }

    public activeInActiveSkid(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Skid/ActiveInActiveSkid`, info
        );
    }

    public validateImportSkid(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>> (
            `${environment.apiUrl}Skid/ValidateImportSkid`,
            formData
        );
    }

    public importSkid(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}Skid/ImportSkid`,
            formData
        );
    }
}