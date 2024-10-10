import { Injectable } from "@angular/core";
import { ZoneInfoDtoModel } from "@c/masters/zone/list-zone-table";
import { BaseSearchHelperService } from "../common";
import { ZoneService } from "./zone.service";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ZoneSearchHelperService extends BaseSearchHelperService<ZoneInfoDtoModel> {
    constructor(private _zoneService: ZoneService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ZoneInfoDtoModel>> {
        return this._zoneService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ZoneInfoDtoModel>
    ): ReadonlyArray<ZoneInfoDtoModel> {
        return response.items ?? [];
    }
}