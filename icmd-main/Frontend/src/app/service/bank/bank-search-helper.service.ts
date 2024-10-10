import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { BankInfoDtoModel } from "@c/masters/bank/list-bank-table";
import { BankService } from "./bank.service";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class BankSearchHelperService extends BaseSearchHelperService<BankInfoDtoModel> {
    constructor(private _bankService: BankService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<BankInfoDtoModel>> {
        return this._bankService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<BankInfoDtoModel>
    ): ReadonlyArray<BankInfoDtoModel> {
        return response.items ?? [];
    }
}