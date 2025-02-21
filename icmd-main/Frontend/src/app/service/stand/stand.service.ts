import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { CreateOrEditStandDtoModel } from "@c/masters/stand/create-edit-stand-form";
import { StandListDtoModel } from "@c/masters/stand/list-stand-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ActiveInActiveDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class StandService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<StandListDtoModel>> {
        return this._http.post<PagedResultModel<StandListDtoModel>>(
            `${environment.apiUrl}Stand/GetAllStands`,
            request
        );
    }

    public getStandInfo(id: string): Observable<CreateOrEditStandDtoModel> {
        return this._http.get<CreateOrEditStandDtoModel>(
            `${environment.apiUrl}Stand/GetStandInfo?id=${id}`
        );
    }

    public createEditStand(info: CreateOrEditStandDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Stand/CreateOrEditStand`,
            info
        );
    }

    public deleteStand(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Stand/DeleteStand?id=${id}`
        );
    }

    public deleteBulkStand(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Stand/DeleteBulkStands`, {
                body: ids
            }
        );
    }

    public activeInActiveStand(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Stand/ActiveInActiveStand`, info
        );
    }

    public validateImportStand(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>> (
            `${environment.apiUrl}Stand/ValidateImportStand`,
            formData
        );
    }

    public importStand(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}Stand/ImportStand`,
            formData
        );
    }
}