import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  AttributeValueDtoModel,
  CreateOrEditDeviceDtoModel,
  DeviceAttributeInfoDtoModel,
  DeviceDropdownInfoDtoModel
} from '@c/manage-device/create-edit-device-form';
import { ViewDeviceInfoDtoModel } from '@c/manage-device/view-device-detail';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import { ActiveInActiveDtoModel } from '@m/common';

@Injectable()
export class DeviceService {
    constructor(private _http: HttpClient) { }

    public getDeviceDropdownInfo(projectId: string, deviceId: string): Observable<DeviceDropdownInfoDtoModel> {
        return this._http.get<DeviceDropdownInfoDtoModel>(
            `${environment.apiUrl}Device/DeviceDropdownInfo?projectId=${projectId}&deviceId=${deviceId}`
        );
    }

    public createOrEditDevice(info: CreateOrEditDeviceDtoModel) {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Device/CreateOrEditDevice`,
            info
        );
    }
    public getDeviceInfoById(id: string): Observable<CreateOrEditDeviceDtoModel> {
        return this._http.get<CreateOrEditDeviceDtoModel>(
            `${environment.apiUrl}Device/GetDeviceInfo?id=${id}`);
    }

    public getAttributes(info: DeviceAttributeInfoDtoModel): Observable<AttributeValueDtoModel[]> {
        return this._http.post<AttributeValueDtoModel[]>(
            `${environment.apiUrl}Device/GetAttributes`, info);
    }

    public viewDeviceInfoById(id: string): Observable<ViewDeviceInfoDtoModel> {
        return this._http.get<ViewDeviceInfoDtoModel>(
            `${environment.apiUrl}Device/ViewDeviceInfo?id=${id}`);
    }

    public deleteDevice(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Device/DeleteDevice?id=${id}`
        );
    }

    public deleteBulkInstrumentDevices(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Device/DeleteBulkInstrumentDevices`, {
                body: ids,
            }
        );
    }

    public deleteBulkNonInstrumentDevices(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Device/DeleteBulkNonInstrumentDevices`, {
                body: ids,
            }
        );
    }

    public activeInActiveDevice(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Device/ActiveInActiveDevice`, info
        );
    }
}