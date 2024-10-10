import { ProjectListDtoModel } from "@c/manage-projects/list-project-table";
import { ProjectService } from "./project.service";
import { BaseSearchHelperService } from "../common";
import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ProjectSearchHelperService extends BaseSearchHelperService<ProjectListDtoModel> {
    constructor(private _projectService: ProjectService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ProjectListDtoModel>> {
        return this._projectService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ProjectListDtoModel>
    ): ReadonlyArray<ProjectListDtoModel> {
        return response.items ?? [];
    }
}