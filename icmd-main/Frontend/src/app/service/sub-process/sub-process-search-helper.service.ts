import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { SubProcessInfoDtoModel } from "@c/masters/sub-process/list-sub-process-table";
import { SubProcessService } from "./sub-process.service";

@Injectable()
export class SubProcessSearchHelperService extends BaseSearchHelperService<SubProcessInfoDtoModel> {
    constructor(private _subProcessService: SubProcessService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SubProcessInfoDtoModel>> {
        return this._subProcessService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<SubProcessInfoDtoModel>
    ): ReadonlyArray<SubProcessInfoDtoModel> {
        return response.items ?? [];
    }
}