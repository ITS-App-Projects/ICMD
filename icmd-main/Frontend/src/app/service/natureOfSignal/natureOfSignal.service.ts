import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditNatureOfSignalDtoModel } from '@c/masters/natureOfSignal/create-edit-natureOfSignal-form';
import { NatureOfSignalListDtoModel } from '@c/masters/natureOfSignal/list-natureOfSignal-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';
import { NatureOfSignalExportDtoModel } from '@p/admin/masters/natureOfSignal/list-natureOfSignal-page';

@Injectable()
export class NatureOfSignalService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<NatureOfSignalListDtoModel>> {
        return this._http.post<PagedResultModel<NatureOfSignalListDtoModel>>(
            `${environment.apiUrl}NatureOfSignal/GetAllNatureOfSignals`,
            request
        );
    }

    public getNatureOfSignalInfo(id: string): Observable<CreateOrEditNatureOfSignalDtoModel> {
        return this._http.get<CreateOrEditNatureOfSignalDtoModel>(
            `${environment.apiUrl}NatureOfSignal/GetNatureOfSignalInfo?id=${id}`
        );
    }

    public createEditNatureOfSignal(info: CreateOrEditNatureOfSignalDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}NatureOfSignal/CreateOrEditNatureOfSignal`,
            info
        );
    }

    public deleteNatureOfSignal(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}NatureOfSignal/DeleteNatureOfSignal?id=${id}`
        );
    }

    public deleteBulkNatureOfSignal(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}NatureOfSignal/DeleteBulkNatureOfSignals`, {
                body: ids
            }
        );
    }
    
    public validateImportNatureOfSignal(file: File): Observable<ImportFileResultModel<NatureOfSignalExportDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<NatureOfSignalExportDtoModel>> (
            `${environment.apiUrl}NatureOfSignal/ValidateImportNatureOfSignal`,
            formData
        );
    }

    public importNatureOfSignal(file: File): Observable<ImportFileResultModel<NatureOfSignalExportDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<NatureOfSignalExportDtoModel>>(
            `${environment.apiUrl}NatureOfSignal/ImportNatureOfSignal`,
            formData
        );
    }

}