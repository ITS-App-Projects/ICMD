import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { CableListService } from "./cable-list.service";
import { ViewCableListDtoModel } from "@c/cableList/list-cableList-table";

@Injectable()
export class CableListSearchHelperService extends BaseSearchHelperService<ViewCableListDtoModel> {
    constructor(private _cableListService: CableListService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewCableListDtoModel>> {
        return this._cableListService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ViewCableListDtoModel>
    ): ReadonlyArray<ViewCableListDtoModel> {
        return response.items ?? [];
    }
}