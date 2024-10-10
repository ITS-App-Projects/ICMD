import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { FailStateInfoDtoModel } from "@c/masters/failState/list-failState-table";
import { FailStateService } from "./failState.service";

@Injectable()
export class FailStateSearchHelperService extends BaseSearchHelperService<FailStateInfoDtoModel> {
    constructor(private _failStateService: FailStateService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<FailStateInfoDtoModel>> {
        return this._failStateService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<FailStateInfoDtoModel>
    ): ReadonlyArray<FailStateInfoDtoModel> {
        return response.items ?? [];
    }
}