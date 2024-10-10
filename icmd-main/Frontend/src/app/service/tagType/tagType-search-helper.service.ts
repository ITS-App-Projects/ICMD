import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { TagTypeInfoDtoModel } from "@c/masters/tagType/list-tagType-table";
import { TagTypeService } from "./tagType.service";

@Injectable()
export class TagTypeSearchHelperService extends BaseSearchHelperService<TagTypeInfoDtoModel> {
    constructor(private _tagTypeService: TagTypeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagTypeInfoDtoModel>> {
        return this._tagTypeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<TagTypeInfoDtoModel>
    ): ReadonlyArray<TagTypeInfoDtoModel> {
        return response.items ?? [];
    }
}