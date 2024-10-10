import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { ProcessInfoDtoModel } from "@c/masters/process/list-process-table";
import { ProcessService } from "./process.service";

@Injectable()
export class ProcessSearchHelperService extends BaseSearchHelperService<ProcessInfoDtoModel> {
    constructor(private _processService: ProcessService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ProcessInfoDtoModel>> {
        return this._processService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ProcessInfoDtoModel>
    ): ReadonlyArray<ProcessInfoDtoModel> {
        return response.items ?? [];
    }
}