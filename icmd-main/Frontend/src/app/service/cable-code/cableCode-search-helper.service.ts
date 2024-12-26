import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { CableCodeService } from "./cableCode.service";
import { CableCodeListDtoModel } from "@c/masters/cable-code/list-cable-code-table/list-cable-code-table.model";

@Injectable()
export class CableCodeSearchHelperService extends BaseSearchHelperService<CableCodeListDtoModel> {
    constructor(private _cableCodeService: CableCodeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<CableCodeListDtoModel>> {
        return this._cableCodeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<CableCodeListDtoModel>
    ): ReadonlyArray<CableCodeListDtoModel> {
        return response.items ?? [];
    }
}