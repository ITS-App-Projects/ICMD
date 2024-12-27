import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { DesignPackageService } from "./designPackage.service";
import { DesignPackageListDtoModel } from "@c/masters/design-package/list-design-package-table";

@Injectable()
export class DesignPackageSearchHelperService extends BaseSearchHelperService<DesignPackageListDtoModel> {
    constructor(private _designPackageService: DesignPackageService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<DesignPackageListDtoModel>> {
        return this._designPackageService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<DesignPackageListDtoModel>
    ): ReadonlyArray<DesignPackageListDtoModel> {
        return response.items ?? [];
    }
}