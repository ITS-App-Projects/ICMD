import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditJunctionBoxDtoModel } from "@c/masters/junction-box/create-edit-junction-box-form";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ActiveInActiveDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class PanelService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._http.post<PagedResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}Panel/GetAllPanels`,
            request
        );
    }

    public getPanelnfo(id: string): Observable<CreateOrEditJunctionBoxDtoModel> {
        return this._http.get<CreateOrEditJunctionBoxDtoModel>(
            `${environment.apiUrl}Panel/GetPanelInfo?id=${id}`
        );
    }

    public createEditPanel(info: CreateOrEditJunctionBoxDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Panel/CreateOrEditPanel`,
            info
        );
    }

    public deletePanel(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Panel/DeletePanel?id=${id}`
        );
    }

    public deleteBulkPanel(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Panel/DeleteBulkPanels`, {
                body: ids
            }
        );
    }

    public activeInActivePanel(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Panel/ActiveInActivePanel`, info
        );
    }

    public validateImportPanel(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>> (
            `${environment.apiUrl}Panel/ValidateImportPanel`,
            formData
        );
    }

    public importPanel(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}Panel/ImportPanel`,
            formData
        );
    }
}