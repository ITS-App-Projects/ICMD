import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { SystemService } from "./system.service";

@Injectable()
export class SystemSearchHelperService extends BaseSearchHelperService<SystemInfoDtoModel> {
    constructor(private _systemService: SystemService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<SystemInfoDtoModel>> {
        return this._systemService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<SystemInfoDtoModel>
    ): ReadonlyArray<SystemInfoDtoModel> {
        return response.items ?? [];
    }
}