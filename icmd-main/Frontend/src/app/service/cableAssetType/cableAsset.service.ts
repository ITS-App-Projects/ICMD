import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { Observable } from "rxjs";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";

import { CableAssetListDtoModel } from "@c/masters/cable-asset-type/list-cable-asset-table";
import { CreateOrEditCableAssetDtoModel } from "@c/masters/cable-asset-type/create-edit-cableAsset-form/create-edit-cableAsset-form.model";

@Injectable()
export class CableAssetService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableAssetListDtoModel>> {
        return this._http.post<PagedResultModel<CableAssetListDtoModel>>(
            `${environment.apiUrl}CableAssetType/GetAllCableAssetTypes`,
            request
        );
    }

    public getCableAssetInfo(id: string): Observable<CreateOrEditCableAssetDtoModel> {
        return this._http.get<CreateOrEditCableAssetDtoModel>(
            `${environment.apiUrl}CableAssetType/GetCableAssetTypeInfo?id=${id}`
        );
    }

    public createEditCableAsset(info: CreateOrEditCableAssetDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/CreateOrEditCableAsset`,
            info
        );
    }

    public deleteCableAsset(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/DeleteCableAssetType?id=${id}`
        );
    }

    public importCableAsset(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}CableAssetType/ImportCableAssetType`,
            formData
        );
    }
}