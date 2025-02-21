import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditJunctionBoxDtoModel } from "@c/masters/junction-box/create-edit-junction-box-form";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ActiveInActiveDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class JunctionBoxService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._http.post<PagedResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}JunctionBox/GetAllJunctionBoxes`,
            request
        );
    }

    public getJunctionBoxInfo(id: string): Observable<CreateOrEditJunctionBoxDtoModel> {
        return this._http.get<CreateOrEditJunctionBoxDtoModel>(
            `${environment.apiUrl}JunctionBox/GetJunctionBoxInfo?id=${id}`
        );
    }

    public createEditJunctionBox(info: CreateOrEditJunctionBoxDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}JunctionBox/CreateOrEditJunctionBox`,
            info
        );
    }

    public deleteJunctionBox(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}JunctionBox/DeleteJunctionBox?id=${id}`
        );
    }

    public deleteBulkJunction(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}JunctionBox/DeleteBulkJunctionBoxes`, {
                body: ids,
            }
        );
    }

    public activeInActiveJunctionBox(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}JunctionBox/ActiveInActiveJunctionBox`, info
        );
    }

    public validateImportJunctionBox(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>> (
            `${environment.apiUrl}JunctionBox/ValidateImportJunctionBox`,
            formData
        );
    }

    public importJunctionBox(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}JunctionBox/ImportJunctionBox`,
            formData
        );
    }
}