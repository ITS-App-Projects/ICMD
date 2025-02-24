import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditReferenceDocumentDtoModel } from "@c/masters/reference-document/create-edit-reference-document-form";
import { ReferenceDocumentInfoDtoModel } from "@c/masters/reference-document/list-reference-document-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { DropdownInfoDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ReferenceDocumentService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ReferenceDocumentInfoDtoModel>> {
        return this._http.post<PagedResultModel<ReferenceDocumentInfoDtoModel>>(
            `${environment.apiUrl}ReferenceDocument/GetAllReferenceDocument`,
            request
        );
    }

    public getReferenceDocumentInfo(id: string): Observable<ReferenceDocumentInfoDtoModel> {
        return this._http.get<ReferenceDocumentInfoDtoModel>(
            `${environment.apiUrl}ReferenceDocument/GetReferenceDocumentInfo?id=${id}`
        );
    }

    public createEditReferenceDocument(info: CreateOrEditReferenceDocumentDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocument/CreateOrEditReferenceDocument`,
            info
        );
    }

    public deleteReferenceDocument(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocument/DeleteReferenceDocument?id=${id}`
        );
    }

    public deleteBulkReferenceDocument(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}ReferenceDocument/DeleteBulkReferenceDocuments`, {
                body: ids,
            }
        );
    }

    public getAllDocumentInfo(projectId: string, referenceDocumentTypeId: string): Observable<DropdownInfoDtoModel[]> {
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}ReferenceDocument/GetAllDocumentInfo?projectId=${projectId}&referenceDocumentTypeId=${referenceDocumentTypeId}`
        );
    }

    public validateImportRefDocument(projectId: string, file: File): Observable<ImportFileResultModel<ReferenceDocumentInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ReferenceDocumentInfoDtoModel>> (
            `${environment.apiUrl}ReferenceDocument/ValidateImportReferenceDocument`,
            formData
        );
    }

    public importReferenceDocument(projectId: string, file: File): Observable<ImportFileResultModel<ReferenceDocumentInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<ReferenceDocumentInfoDtoModel>>(
            `${environment.apiUrl}ReferenceDocument/ImportReferenceDocument`,
            formData
        );
    }
}