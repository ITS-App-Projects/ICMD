import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { SubSystemInfoDtoModel } from "@c/masters/sub-system/list-sub-system-table";
import { SubSystemService } from "./sub-system.service";

@Injectable()
export class SubSystemSearchHelperService extends BaseSearchHelperService<SubSystemInfoDtoModel> {
    constructor(private _subSystemService: SubSystemService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SubSystemInfoDtoModel>> {
        return this._subSystemService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<SubSystemInfoDtoModel>
    ): ReadonlyArray<SubSystemInfoDtoModel> {
        return response.items ?? [];
    }
}