import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { CableAssetService } from "./cableAsset.service";
import { CableAssetListDtoModel } from "@c/masters/cable-asset-type/list-cable-asset-table";

@Injectable()
export class CableAssetSearchHelperService extends BaseSearchHelperService<CableAssetListDtoModel> {
    constructor(private _cableAssetService: CableAssetService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableAssetListDtoModel>> {
        return this._cableAssetService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<CableAssetListDtoModel>
    ): ReadonlyArray<CableAssetListDtoModel> {
        return response.items ?? [];
    }
}