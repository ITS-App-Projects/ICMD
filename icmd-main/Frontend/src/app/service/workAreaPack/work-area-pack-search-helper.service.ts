import { Injectable } from "@angular/core";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";
import { WorkAreaPackService } from "./workAreaPack.service";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class WorkAreaPackSearchHelperService extends BaseSearchHelperService<WorkAreaPackInfoDtoModel> {
    constructor(private _workAreaPackService: WorkAreaPackService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<WorkAreaPackInfoDtoModel>> {
        return this._workAreaPackService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<WorkAreaPackInfoDtoModel>
    ): ReadonlyArray<WorkAreaPackInfoDtoModel> {
        return response.items ?? [];
    }
}