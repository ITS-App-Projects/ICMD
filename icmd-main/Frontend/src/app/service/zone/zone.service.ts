import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditZoneDtoModel } from "@c/masters/zone/create-edit-zone-form";
import { ZoneInfoDtoModel } from "@c/masters/zone/list-zone-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ZoneService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ZoneInfoDtoModel>> {
        return this._http.post<PagedResultModel<ZoneInfoDtoModel>>(
            `${environment.apiUrl}Zone/GetAllZones`,
            request
        );
    }

    public getZoneInfo(id: string): Observable<ZoneInfoDtoModel> {
        return this._http.get<ZoneInfoDtoModel>(
            `${environment.apiUrl}Zone/GetZoneInfo?id=${id}`
        );
    }

    public createEditZone(info: CreateOrEditZoneDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Zone/CreateOrEditZone`,
            info
        );
    }

    public deleteZone(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Zone/DeleteZone?id=${id}`
        );
    }

    public deleteBulkZone(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Zone/DeleteBulkZones`, {
                body: ids,
            }
        );
    }

    public validateImportZone(projectId: string, file: File): Observable<ImportFileResultModel<ZoneInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ZoneInfoDtoModel>> (
            `${environment.apiUrl}Zone/ValidateImportZone`,
            formData
        );
    }

    public importZone(projectId: string, file: File): Observable<ImportFileResultModel<ZoneInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ZoneInfoDtoModel>>(
            `${environment.apiUrl}Zone/ImportZone`,
            formData
        );
    }
}