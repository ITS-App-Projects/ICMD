import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { CableTypeService } from "./cableType.service";
import { CableTypeListDtoModel } from "@c/masters/cable-type/list-cable-type-table";

@Injectable()
export class CableTypeSearchHelperService extends BaseSearchHelperService<CableTypeListDtoModel> {
    constructor(private _cableTypeService: CableTypeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableTypeListDtoModel>> {
        return this._cableTypeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<CableTypeListDtoModel>
    ): ReadonlyArray<CableTypeListDtoModel> {
        return response.items ?? [];
    }
}