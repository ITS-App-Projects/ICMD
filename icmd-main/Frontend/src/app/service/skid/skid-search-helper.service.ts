import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { SkidService } from "./skid.service";

@Injectable()
export class SkidSearchHelperService extends BaseSearchHelperService<JunctionBoxListDtoModel> {
    constructor(private _skidService: SkidService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<JunctionBoxListDtoModel>> {
        return this._skidService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<JunctionBoxListDtoModel>
    ): ReadonlyArray<JunctionBoxListDtoModel> {
        return response.items ?? [];
    }
}