import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { ManufacturerInfoDtoModel } from "@c/masters/manufacturer/list-manufacturer-table";
import { ManufacturerService } from "./manufacturer.service";

@Injectable()
export class ManufacturerSearchHelperService extends BaseSearchHelperService<ManufacturerInfoDtoModel> {
    constructor(private _manufacturerService: ManufacturerService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ManufacturerInfoDtoModel>> {
        return this._manufacturerService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ManufacturerInfoDtoModel>
    ): ReadonlyArray<ManufacturerInfoDtoModel> {
        return response.items ?? [];
    }
}