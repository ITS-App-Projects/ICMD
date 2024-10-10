import { Injectable } from "@angular/core";
import { TrainInfoDtoModel } from "@c/masters/train/list-train-table";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { BaseSearchHelperService } from "../common";
import { TrainService } from "./train.service";

@Injectable()
export class TrainSearchHelperService extends BaseSearchHelperService<TrainInfoDtoModel> {
    constructor(private _trainService: TrainService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TrainInfoDtoModel>> {
        return this._trainService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<TrainInfoDtoModel>
    ): ReadonlyArray<TrainInfoDtoModel> {
        return response.items ?? [];
    }
}