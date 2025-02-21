import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrEditReferenceDocumentTypeModel } from '@c/masters/documentType/create-edit-document-type-form';
import { TypeInfoDtoModel } from '@c/masters/documentType/list-document-type-table';
import { environment } from '@env/environment';
import { BaseResponseModel } from '@m/auth/login-response-model';
import {
  DropdownInfoDtoModel,
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class DocumentTypeService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TypeInfoDtoModel>> {
        return this._http.post<PagedResultModel<TypeInfoDtoModel>>(
            `${environment.apiUrl}ReferenceDocumentType/GetAllDocumentTypes`,
            request
        );
    }

    public getDocumentTypeInfo(id: string): Observable<TypeInfoDtoModel> {
        return this._http.get<TypeInfoDtoModel>(
            `${environment.apiUrl}ReferenceDocumentType/GetDocumentTypeInfo?id=${id}`
        );
    }

    public createEditDocumentType(info: CreateOrEditReferenceDocumentTypeModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocumentType/CreateOrEditReferenceDocumentType`,
            info
        );
    }

    public deleteDocumentType(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocumentType/DeleteDocumentType?id=${id}`
        );
    }


    public deleteBulkDocumentType(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocumentType/DeleteBulkDocumentTypes`, {
                body: ids
            }
        );
    }

    public getAllDocumentTypeInfo(): Observable<DropdownInfoDtoModel[]> {
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}ReferenceDocumentType/GetAllDocumentTypeInfo`
        );
    }

    public validateImportReferenceDocumentType(file: File): Observable<ImportFileResultModel<TypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TypeInfoDtoModel>> (
            `${environment.apiUrl}ReferenceDocumentType/ValidateImportReferenceDocumentType`,
            formData
        );
    }

    public importReferenceDocumentType(file: File): Observable<ImportFileResultModel<TypeInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<TypeInfoDtoModel>>(
            `${environment.apiUrl}ReferenceDocumentType/ImportReferenceDocumentType`,
            formData
        );
    }
}