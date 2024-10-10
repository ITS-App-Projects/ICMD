import { Injectable } from "@angular/core";
import { ViewInstrumentListLiveModel } from "@c/instrument-list/list-instrument-table";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { InstrumentService } from "./instrument.service";

@Injectable()
export class InstrumentSearchHelperService extends BaseSearchHelperService<ViewInstrumentListLiveModel> {
    constructor(private _instrumentService: InstrumentService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewInstrumentListLiveModel>> {
        return this._instrumentService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ViewInstrumentListLiveModel>
    ): ReadonlyArray<ViewInstrumentListLiveModel> {
        return response.items ?? [];
    }
}