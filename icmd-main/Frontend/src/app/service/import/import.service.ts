import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { Observable } from "rxjs";

@Injectable()
export class ImportService {
    constructor(private _http: HttpClient) { }

    public uploadOMItem(projectId: string, file: File): Observable<BaseResponseModel> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);


        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Import/UploadOMItems`,
            formData
        );
    }

    public uploadPIDs(projectId: string, file: File): Observable<BaseResponseModel> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);


        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Import/UploadPnIDs`,
            formData
        );
    }

    public uploadCCMD(projectId: string, file: File): Observable<BaseResponseModel> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);


        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Import/UploadCCMD`,
            formData
        );
    }
}