import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { StandListDtoModel } from "@c/masters/stand/list-stand-table";
import { StandService } from "./stand.service";

@Injectable()
export class StandSearchHelperService extends BaseSearchHelperService<StandListDtoModel> {
    constructor(private _standService: StandService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<StandListDtoModel>> {
        return this._standService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<StandListDtoModel>
    ): ReadonlyArray<StandListDtoModel> {
        return response.items ?? [];
    }
}