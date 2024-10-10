import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { PanelService } from "./panel.service";

@Injectable()
export class PanelSearchHelperService extends BaseSearchHelperService<JunctionBoxListDtoModel> {
    constructor(private _panelService: PanelService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._panelService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<JunctionBoxListDtoModel>
    ): ReadonlyArray<JunctionBoxListDtoModel> {
        return response.items ?? [];
    }
}