import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { Observable } from "rxjs";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";

import { CableSubListDtoModel } from "@c/masters/cable-sub-type/list-cable-sub-table";
import { CreateOrEditCableSubDtoModel } from "@c/masters/cable-sub-type/create-edit-cableSub-form/create-edit-cableSub-form.model";

@Injectable()
export class CableSubService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableSubListDtoModel>> {
        return this._http.post<PagedResultModel<CableSubListDtoModel>>(
            `${environment.apiUrl}CableAssetType/GetAllCableAssetTypes`,
            request
        );
    }

    public getCableSubInfo(id: string): Observable<CreateOrEditCableSubDtoModel> {
        return this._http.get<CreateOrEditCableSubDtoModel>(
            `${environment.apiUrl}CableAssetType/GetCableAssetTypeInfo?id=${id}`
        );
    }

    public createEditCableSub(info: CreateOrEditCableSubDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/CreateOrEditCableAsset`,
            info
        );
    }

    public deleteCableSub(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/DeleteCableAssetType?id=${id}`
        );
    }

    public importCableSub(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}CableAssetType/ImportCableAssetType`,
            formData
        );
    }
}