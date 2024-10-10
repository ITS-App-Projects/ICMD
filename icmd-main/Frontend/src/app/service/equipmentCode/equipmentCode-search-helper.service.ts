import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { EquipmentCodeInfoDtoModel } from "@c/masters/equipmentCode/list-equipmentCode-table";
import { EquipmentCodeService } from "./equipmentCode.service";

@Injectable()
export class EquipmentCodeSearchHelperService extends BaseSearchHelperService<EquipmentCodeInfoDtoModel> {
    constructor(private _equipmentCodeService: EquipmentCodeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<EquipmentCodeInfoDtoModel>> {
        return this._equipmentCodeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<EquipmentCodeInfoDtoModel>
    ): ReadonlyArray<EquipmentCodeInfoDtoModel> {
        return response.items ?? [];
    }
}