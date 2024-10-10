import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { NonInstrumentService } from "./non-instrument.service";
import { ViewNonInstrumentListDtoModel } from "@c/nonInstrument-list/list-nonInstrument-table";

@Injectable()
export class NonInstrumentSearchHelperService extends BaseSearchHelperService<ViewNonInstrumentListDtoModel> {
    constructor(private _nonInstrumentService: NonInstrumentService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewNonInstrumentListDtoModel>> {
        return this._nonInstrumentService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ViewNonInstrumentListDtoModel>
    ): ReadonlyArray<ViewNonInstrumentListDtoModel> {
        return response.items ?? [];
    }
}