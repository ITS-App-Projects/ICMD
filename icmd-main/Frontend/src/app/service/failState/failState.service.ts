import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditFailStateDtoModel } from '@c/masters/failState/create-edit-failState-form';
import { FailStateInfoDtoModel } from '@c/masters/failState/list-failState-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class FailStateService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<FailStateInfoDtoModel>> {
        return this._http.post<PagedResultModel<FailStateInfoDtoModel>>(
            `${environment.apiUrl}FailState/GetAllFailStates`,
            request
        );
    }

    public getFailStateInfo(id: string): Observable<FailStateInfoDtoModel> {
        return this._http.get<FailStateInfoDtoModel>(
            `${environment.apiUrl}FailState/GetFailStateInfo?id=${id}`
        );
    }

    public createEditFailState(info: CreateOrEditFailStateDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}FailState/CreateOrEditFailState`,
            info
        );
    }

    public deleteFailState(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}FailState/DeleteFailState?id=${id}`
        );
    }

    public deleteBulkFailState(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}FailState/DeleteBulkFailStates`, {
                body: ids
            }
        );
    }

    public validateImportFailState(file: File): Observable<ImportFileResultModel<FailStateInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<FailStateInfoDtoModel>> (
            `${environment.apiUrl}FailState/ValidateImportFailState`,
            formData
        );
    }

    public importFailState(file: File): Observable<ImportFileResultModel<FailStateInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<FailStateInfoDtoModel>>(
            `${environment.apiUrl}FailState/ImportFailState`,
            formData
        );
    }
}