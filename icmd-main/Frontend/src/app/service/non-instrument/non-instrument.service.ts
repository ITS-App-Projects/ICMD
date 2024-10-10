import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ViewNonInstrumentListDtoModel } from "@c/nonInstrument-list/list-nonInstrument-table";
import { environment } from "@env/environment";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class NonInstrumentService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewNonInstrumentListDtoModel>> {
        return this._http.post<PagedResultModel<ViewNonInstrumentListDtoModel>>(
            `${environment.apiUrl}NonInstrument/GetAllNonInstruments`,
            request
        );
    }
}