import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { TagTypeInfoDtoModel } from "@c/masters/tagType/list-tagType-table";
import { TagDescriptorService } from "./tagDescriptor.service";

@Injectable()
export class TagDescriptorSearchHelperService extends BaseSearchHelperService<TagTypeInfoDtoModel> {
    constructor(private _tagDescriptorService: TagDescriptorService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TagTypeInfoDtoModel>> {
        return this._tagDescriptorService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<TagTypeInfoDtoModel>
    ): ReadonlyArray<TagTypeInfoDtoModel> {
        return response.items ?? [];
    }
}