import { ListUserTableModel } from "@c/manange-user/list-user-table";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { BaseSearchHelperService } from "../common";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { UserService } from "./user.service";

@Injectable()
export class UserSearchHelperService extends BaseSearchHelperService<ListUserTableModel> {
    constructor(private _userService: UserService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ListUserTableModel>> {
        return this._userService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ListUserTableModel>
    ): ReadonlyArray<ListUserTableModel> {
        return response.items ?? [];
    }
}