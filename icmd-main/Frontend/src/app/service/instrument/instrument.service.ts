import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ViewInstrumentListLiveModel } from "@c/instrument-list/list-instrument-table";
import { environment } from "@env/environment";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class InstrumentService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewInstrumentListLiveModel>> {
        return this._http.post<PagedResultModel<ViewInstrumentListLiveModel>>(
            `${environment.apiUrl}Instrument/GetAllInstruments`,
            request
        );
    }
}