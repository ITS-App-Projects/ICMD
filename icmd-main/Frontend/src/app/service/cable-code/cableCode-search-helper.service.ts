import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { CableSubService } from "./cableSub.service";
import { CableSubListDtoModel } from "@c/masters/cable-sub-type/list-cable-sub-table";

@Injectable()
export class CableSubSearchHelperService extends BaseSearchHelperService<CableSubListDtoModel> {
    constructor(private _cableSubService: CableSubService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableSubListDtoModel>> {
        return this._cableSubService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<CableSubListDtoModel>
    ): ReadonlyArray<CableSubListDtoModel> {
        return response.items ?? [];
    }
}