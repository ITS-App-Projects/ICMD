import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { NatureOfSignalListDtoModel } from "@c/masters/natureOfSignal/list-natureOfSignal-table";
import { NatureOfSignalService } from "./natureOfSignal.service";

@Injectable()
export class NatureOfSignalSearchHelperService extends BaseSearchHelperService<NatureOfSignalListDtoModel> {
    constructor(private _natureOfSignalService: NatureOfSignalService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<NatureOfSignalListDtoModel>> {
        return this._natureOfSignalService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<NatureOfSignalListDtoModel>
    ): ReadonlyArray<NatureOfSignalListDtoModel> {
        return response.items ?? [];
    }
}