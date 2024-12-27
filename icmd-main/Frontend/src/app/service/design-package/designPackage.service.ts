import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { Observable } from "rxjs";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";

import { DesignPackageListDtoModel } from "@c/masters/design-package/list-design-package-table";
import { CreateOrEditDesignPackageDtoModel } from "@c/masters/design-package/create-edit-designPackage-form/create-edit-designPackage-form.model";


@Injectable()
export class DesignPackageService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DesignPackageListDtoModel>> {
        return this._http.post<PagedResultModel<DesignPackageListDtoModel>>(
            `${environment.apiUrl}CableAssetType/GetAllCableAssetTypes`,
            request
        );
    }

    public getDesignPackageInfo(id: string): Observable<CreateOrEditDesignPackageDtoModel> {
        return this._http.get<CreateOrEditDesignPackageDtoModel>(
            `${environment.apiUrl}CableAssetType/GetCableAssetTypeInfo?id=${id}`
        );
    }

    public createEditDesignPackage(info: CreateOrEditDesignPackageDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/CreateOrEditCableAsset`,
            info
        );
    }

    public deleteDesignPackage(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/DeleteCableAssetType?id=${id}`
        );
    }

    public importDesignPackage(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}CableAssetType/ImportCableAssetType`,
            formData
        );
    }
}