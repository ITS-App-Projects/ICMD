import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { DeviceModelListDtoModel } from "@c/masters/device-model/list-device-model-table";
import { DeviceModelService } from "./device-model.service";

@Injectable()
export class DeviceModelSearchHelperService extends BaseSearchHelperService<DeviceModelListDtoModel> {
    constructor(private _deviceModelService: DeviceModelService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DeviceModelListDtoModel>> {
        return this._deviceModelService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<DeviceModelListDtoModel>
    ): ReadonlyArray<DeviceModelListDtoModel> {
        return response.items ?? [];
    }
}