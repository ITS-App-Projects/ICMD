import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditDeviceModelDtoModel } from '@c/masters/device-model/create-edit-device-model-form';
import { DeviceModelListDtoModel } from '@c/masters/device-model/list-device-model-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  DropdownInfoDtoModel,
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class DeviceModelService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DeviceModelListDtoModel>> {
        return this._http.post<PagedResultModel<DeviceModelListDtoModel>>(
            `${environment.apiUrl}DeviceModel/GetAllDeviceModels`,
            request
        );
    }

    public getDeviceModelInfo(id: string): Observable<CreateOrEditDeviceModelDtoModel> {
        return this._http.get<CreateOrEditDeviceModelDtoModel>(
            `${environment.apiUrl}DeviceModel/GetDeviceModelInfo?id=${id}`
        );
    }

    public createEditDeviceModel(info: CreateOrEditDeviceModelDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}DeviceModel/CreateOrEditDeviceModel`,
            info
        );
    }

    public deleteDeviceModel(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}DeviceModel/DeleteDeviceModel?id=${id}`
        );
    }

    public deleteBulkDeviceModel(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}DeviceModel/DeleteBulkDeviceModels`, {
                body: ids
            }
        );
    }

    public getDeviceInfoFromManufacturerId(manufacturerId: string): Observable<DropdownInfoDtoModel[]> {
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}DeviceModel/GetDeviceInfoFromManufacturerId?manufacturerId=${manufacturerId}`
        );
    }

    public validateImportDeviceModel(file: File): Observable<ImportFileResultModel<DeviceModelListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<DeviceModelListDtoModel>> (
            `${environment.apiUrl}DeviceModel/ValidateImportDeviceModel`,
            formData
        );
    }

    public importDeviceModel(file: File): Observable<ImportFileResultModel<DeviceModelListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<DeviceModelListDtoModel>>(
            `${environment.apiUrl}DeviceModel/ImportDeviceModel`,
            formData
        );
    }

}