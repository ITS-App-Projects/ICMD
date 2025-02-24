import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditStreamDtoModel } from "@c/masters/stream/create-edit-stream-form";
import { StreamInfoDtoModel } from "@c/masters/stream/list-stream-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class StreamService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<StreamInfoDtoModel>> {
        return this._http.post<PagedResultModel<StreamInfoDtoModel>>(
            `${environment.apiUrl}Stream/GetAllStreams`,
            request
        );
    }

    public getStreamInfo(id: string): Observable<StreamInfoDtoModel> {
        return this._http.get<StreamInfoDtoModel>(
            `${environment.apiUrl}Stream/GetStreamInfo?id=${id}`
        );
    }

    public createEditStream(info: CreateOrEditStreamDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Stream/CreateOrEditStream`,
            info
        );
    }

    public deleteStream(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Stream/DeleteStream?id=${id}`
        );
    }

    public deleteBulkStream(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Stream/DeleteBulkStreams`, {
                body: ids
            }
        );
    }

    public validateImportStream(projectId: string, file: File): Observable<ImportFileResultModel<StreamInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<StreamInfoDtoModel>> (
            `${environment.apiUrl}Stream/ValidateImportStream`,
            formData
        );
    }

    public importStream(projectId: string, file: File): Observable<ImportFileResultModel<StreamInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<StreamInfoDtoModel>>(
            `${environment.apiUrl}Stream/ImportStream`,
            formData
        );
    }
}