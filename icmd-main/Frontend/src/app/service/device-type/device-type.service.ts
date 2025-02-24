import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditDeviceTypeDtoModel } from '@c/masters/device-type/create-edit-device-type-form';
import { DeviceTypeListDtoModel } from '@c/masters/device-type/list-device-type-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class DeviceTypeService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DeviceTypeListDtoModel>> {
        return this._http.post<PagedResultModel<DeviceTypeListDtoModel>>(
            `${environment.apiUrl}DeviceType/GetAllDeviceTypes`,
            request
        );
    }

    public getDeviceTypeInfo(id: string): Observable<CreateOrEditDeviceTypeDtoModel> {
        return this._http.get<CreateOrEditDeviceTypeDtoModel>(
            `${environment.apiUrl}DeviceType/GetDeviceTypeInfo?id=${id}`
        );
    }

    public createEditDeviceType(info: CreateOrEditDeviceTypeDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}DeviceType/CreateOrEditDeviceType`,
            info
        );
    }

    public deleteDeviceType(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}DeviceType/DeleteDeviceType?id=${id}`
        );
    }

    public deleteBulkDeviceType(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}DeviceType/DeleteBulkDeviceTypes`, {
                body: ids
            }
        );
    }

    public validateImportDeviceType(file: File): Observable<ImportFileResultModel<DeviceTypeListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<DeviceTypeListDtoModel>> (
            `${environment.apiUrl}DeviceType/ValidateImportDeviceType`,
            formData
        );
    }

    public importDeviceType(file: File): Observable<ImportFileResultModel<DeviceTypeListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<DeviceTypeListDtoModel>>(
            `${environment.apiUrl}DeviceType/ImportDeviceType`,
            formData
        );
    }
}