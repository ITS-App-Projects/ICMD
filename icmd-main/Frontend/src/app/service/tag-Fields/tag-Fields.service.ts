import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ProjectTagFieldsInfoDtoModel, TagFieldInfoDtoModel } from "@p/admin/masters/tag-Fields/list-tag-Fields-page";
import { Observable } from "rxjs";

@Injectable()
export class TagFieldsService {
    constructor(private _http: HttpClient) { }

    public getProjectTagFieldInfo(id: string): Observable<Array<TagFieldInfoDtoModel>> {
        return this._http.get<Array<TagFieldInfoDtoModel>>(
            `${environment.apiUrl}TagField/GetProjectTagFieldsInfo?projectId=${id}`
        );
    }

    public updateProjectTagFields(info: ProjectTagFieldsInfoDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}TagField/UpdateProjectTagFields`,
            info
        );
    }
}