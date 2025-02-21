import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditTagTypeDescriptionDtoModel } from '@c/masters/tagType/create-edit-tagType-form';
import { TagTypeInfoDtoModel } from '@c/masters/tagType/list-tagType-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class TagDescriptorService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagTypeInfoDtoModel>> {
        return this._http.post<PagedResultModel<TagTypeInfoDtoModel>>(
            `${environment.apiUrl}TagDescriptor/GetAllTagDescriptors`,
            request
        );
    }

    public getTagDescriptorInfo(id: string): Observable<TagTypeInfoDtoModel> {
        return this._http.get<TagTypeInfoDtoModel>(
            `${environment.apiUrl}TagDescriptor/GetTagDescriptorInfo?id=${id}`
        );
    }

    public createEditTagDescriptor(info: CreateOrEditTagTypeDescriptionDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}TagDescriptor/CreateOrEditTagDescriptor`,
            info
        );
    }

    public deleteTagDescriptor(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}TagDescriptor/DeleteTagDescriptor?id=${id}`
        );
    }

    public deleteBulkTagDescriptor(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}TagDescriptor/DeleteBulkTagDescriptors`, {
                body: ids
            }
        );
    }

    public validateImportTagDescriptor(file: File): Observable<ImportFileResultModel<TagTypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TagTypeInfoDtoModel>>(
            `${environment.apiUrl}TagDescriptor/ValidateImportTagDescriptor`,
            formData
        );
    }

    public importTagDescriptor(file: File): Observable<ImportFileResultModel<TagTypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TagTypeInfoDtoModel>>(
            `${environment.apiUrl}TagDescriptor/ImportTagDescriptor`,
            formData
        );
    }
}