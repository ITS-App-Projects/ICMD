import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

import { RoomService } from "./room.service";
import { RoomListDtoModel } from "@c/masters/room/list-room-table";

@Injectable()
export class RoomSearchHelperService extends BaseSearchHelperService<RoomListDtoModel> {
    constructor(private _roomService: RoomService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<RoomListDtoModel>> {
        return this._roomService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<RoomListDtoModel>
    ): ReadonlyArray<RoomListDtoModel> {
        return response.items ?? [];
    }
}