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
export class TagTypeService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagTypeInfoDtoModel>> {
        return this._http.post<PagedResultModel<TagTypeInfoDtoModel>>(
            `${environment.apiUrl}TagType/GetAllTagTypes`,
            request
        );
    }

    public getTagTypeInfo(id: string): Observable<TagTypeInfoDtoModel> {
        return this._http.get<TagTypeInfoDtoModel>(
            `${environment.apiUrl}TagType/GetTagTypeInfo?id=${id}`
        );
    }

    public createEditTagType(info: CreateOrEditTagTypeDescriptionDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}TagType/CreateOrEditTagType`,
            info
        );
    }

    public deleteTagType(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}TagType/DeleteTagType?id=${id}`
        );
    }

    public deleteBulkTagType(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}TagType/DeleteBulkTagTypes`, {
                body: ids
            }
        );
    }

    public validateImportTagType(file: File): Observable<ImportFileResultModel<TagTypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TagTypeInfoDtoModel>> (
            `${environment.apiUrl}TagType/ValidateImportTagType`,
            formData
        );
    }

    public importTagType(file: File): Observable<ImportFileResultModel<TagTypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TagTypeInfoDtoModel>>(
            `${environment.apiUrl}TagType/ImportTagType`,
            formData
        );
    }
}