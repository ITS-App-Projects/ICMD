import { Injectable } from "@angular/core";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { StreamInfoDtoModel } from "@c/masters/stream/list-stream-table";
import { StreamService } from "./stream.service";

@Injectable()
export class StreamSearchHelperService extends BaseSearchHelperService<StreamInfoDtoModel> {
    constructor(private _streamService: StreamService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<StreamInfoDtoModel>> {
        return this._streamService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<StreamInfoDtoModel>
    ): ReadonlyArray<StreamInfoDtoModel> {
        return response.items ?? [];
    }
}