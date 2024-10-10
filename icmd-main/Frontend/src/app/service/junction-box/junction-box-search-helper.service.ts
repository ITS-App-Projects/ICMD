import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { JunctionBoxService } from "./junction-box.service";

@Injectable()
export class JunctionBoxSearchHelperService extends BaseSearchHelperService<JunctionBoxListDtoModel> {
    constructor(private _junctionBoxService: JunctionBoxService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._junctionBoxService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<JunctionBoxListDtoModel>
    ): ReadonlyArray<JunctionBoxListDtoModel> {
        return response.items ?? [];
    }
}