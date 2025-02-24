import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditEquipmentCodeDtoModel } from '@c/masters/equipmentCode/create-edit-equipmentCode-form';
import { EquipmentCodeInfoDtoModel } from '@c/masters/equipmentCode/list-equipmentCode-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class EquipmentCodeService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<EquipmentCodeInfoDtoModel>> {
        return this._http.post<PagedResultModel<EquipmentCodeInfoDtoModel>>(
            `${environment.apiUrl}EquipmentCode/GetAllEquipmentCodes`,
            request
        );
    }

    public getEquipmentCodeInfo(id: string): Observable<EquipmentCodeInfoDtoModel> {
        return this._http.get<EquipmentCodeInfoDtoModel>(
            `${environment.apiUrl}EquipmentCode/GetEquipmentCodeInfo?id=${id}`
        );
    }

    public createEditEquipmentCode(info: CreateOrEditEquipmentCodeDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}EquipmentCode/CreateOrEditEquipmentCode`,
            info
        );
    }

    public deleteEquipmentCode(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}EquipmentCode/DeleteEquipmentCode?id=${id}`
        );
    }

    public deleteBulkEquipmentCode(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}EquipmentCode/DeleteBulkEquipmentCodes`, {
                body: ids
            }
        );
    }

    public validateImportEquipmentCode(file: File): Observable<ImportFileResultModel<EquipmentCodeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<EquipmentCodeInfoDtoModel>> (
            `${environment.apiUrl}EquipmentCode/ValidateImportEquipmentCode`,
            formData
        );
    }

    public importEquipmentCode(file: File): Observable<ImportFileResultModel<EquipmentCodeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<EquipmentCodeInfoDtoModel>>(
            `${environment.apiUrl}EquipmentCode/ImportEquipmentCode`,
            formData
        );
    }
}