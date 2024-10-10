import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditProjectDtoModel } from "@c/manage-projects/create-edit-project-form";
import { ProjectListDtoModel } from "@c/manage-projects/list-project-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ActiveInActiveDtoModel, DropdownInfoDtoModel, PagedAndSortedResultRequestModel, PagedResultModel, ProjectTagFieldInfoDtoModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ProjectService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ProjectListDtoModel>> {
        return this._http.post<PagedResultModel<ProjectListDtoModel>>(
            `${environment.apiUrl}Project/GetAllProjects`,
            request
        );
    }

    public createEditProject(info: CreateOrEditProjectDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Project/CreateOrEditProject`,
            info
        );
    }

    public getProjectInfo(id: string): Observable<CreateOrEditProjectDtoModel> {
        return this._http.get<CreateOrEditProjectDtoModel>(
            `${environment.apiUrl}Project/GetProjectInfo?id=${id}`
        );
    }

    public deleteProject(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Project/DeleteProject?id=${id}`
        );
    }

    public activeInActiveProject(info: ActiveInActiveDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Project/ActiveInActiveProject`, info
        );
    }

    public getAllProjectsInfo(): Observable<DropdownInfoDtoModel[]> {
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}Project/GetAllProjectsInfo`
        );
    }

    public getProjectTagFieldNames(id: string): Observable<string[]> {
        return this._http.get<string[]>(
            `${environment.apiUrl}Project/GetProjectTagFieldNames?id=${id}`
        );
    }

    public getProjectTagFieldSourcesDataInfo(id: string): Observable<ProjectTagFieldInfoDtoModel[]> {
        return this._http.get<ProjectTagFieldInfoDtoModel[]>(
            `${environment.apiUrl}Project/GetProjectTagFieldSourcesDataInfo?id=${id}`
        );
    }

}