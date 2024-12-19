import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { Observable } from "rxjs";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";

import { CableTypeListDtoModel } from "@c/masters/cable-type/list-cable-type-table";
import { CreateOrEditCableTypeDtoModel } from "@c/masters/cable-type/create-edit-cableType-form/create-edit-cableType-form.model";

@Injectable()
export class CableTypeService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableTypeListDtoModel>> {
        return this._http.post<PagedResultModel<CableTypeListDtoModel>>(
            `${environment.apiUrl}CableAssetType/GetAllCableAssetTypes`,
            request
        );
    }

    public getCableTypeInfo(id: string): Observable<CreateOrEditCableTypeDtoModel> {
        return this._http.get<CreateOrEditCableTypeDtoModel>(
            `${environment.apiUrl}CableAssetType/GetCableAssetTypeInfo?id=${id}`
        );
    }

    public createEditCableType(info: CreateOrEditCableTypeDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/CreateOrEditCableAsset`,
            info
        );
    }

    public deleteCableType(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/DeleteCableAssetType?id=${id}`
        );
    }

    public importCableType(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}CableAssetType/ImportCableAssetType`,
            formData
        );
    }
}