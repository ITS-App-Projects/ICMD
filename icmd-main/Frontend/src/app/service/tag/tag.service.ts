import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditTagDtoModel, GenerateTagDtoModel } from "@c/masters/tag/create-edit-tag-form";
import { TagListDtoModel } from "@c/masters/tag/list-tag-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { DropdownInfoDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class TagService {
    constructor(private _http: HttpClient) { }
    private emptyGuid: string = "00000000-0000-0000-0000-000000000000";

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagListDtoModel>> {
        return this._http.post<PagedResultModel<TagListDtoModel>>(
            `${environment.apiUrl}Tag/GetAllTags`,
            request
        );
    }

    public getTagInfo(id: string): Observable<CreateOrEditTagDtoModel> {
        return this._http.get<CreateOrEditTagDtoModel>(
            `${environment.apiUrl}Tag/GetTagInfo?id=${id}`
        );
    }

    public createEditTag(info: CreateOrEditTagDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Tag/CreateOrEditTag`,
            info
        );
    }

    public deleteTag(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Tag/DeleteTag?id=${id}`
        );
    }

    public deleteBulkTag(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Tag/DeleteBulkTags`, {
                body: ids,
            }
        );
    }

    public generateTag(info: GenerateTagDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Tag/GenerateTag`,
            info
        );
    }

    public getProjectWiseTagInfo(id: string, type: string, entityId: string = this.emptyGuid): Observable<DropdownInfoDtoModel[]> {
        entityId = entityId ? entityId : this.emptyGuid;
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}Tag/GetProjectWiseTagInfo?projectId=${id}&type=${type}&id=${entityId}`
        );
    }
    
    public validateImportTag(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>> (
            `${environment.apiUrl}Tag/ValidateImportTag`,
            formData
        );
    }

    public importTag(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>>(
            `${environment.apiUrl}Tag/ImportTag`,
            formData
        );
    }
}