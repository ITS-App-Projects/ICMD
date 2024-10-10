import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { TagListDtoModel } from "@c/masters/tag/list-tag-table";
import { TagService } from "./tag.service";

@Injectable()
export class TagSearchHelperService extends BaseSearchHelperService<TagListDtoModel> {
    constructor(private _tagService: TagService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagListDtoModel>> {
        return this._tagService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<TagListDtoModel>
    ): ReadonlyArray<TagListDtoModel> {
        return response.items ?? [];
    }
}