import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { DeviceTypeListDtoModel } from "@c/masters/device-type/list-device-type-table";
import { DeviceTypeService } from "./device-type.service";

@Injectable()
export class DeviceTypeSearchHelperService extends BaseSearchHelperService<DeviceTypeListDtoModel> {
    constructor(private _deviceTypeService: DeviceTypeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DeviceTypeListDtoModel>> {
        return this._deviceTypeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<DeviceTypeListDtoModel>
    ): ReadonlyArray<DeviceTypeListDtoModel> {
        return response.items ?? [];
    }
}