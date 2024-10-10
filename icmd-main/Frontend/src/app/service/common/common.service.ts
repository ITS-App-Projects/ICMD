import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@env/environment";
import { AddMemoryCacheModel } from "@m/common";
import { InstrumentDropdownInfoDtoModel } from "@p/admin/instrument-list/list-instrument-page";
import { NonInstrumentDropdownInfoDtoModel } from "@p/admin/nonInstrument-list/list-nonInstrument-page";
import { Observable } from "rxjs";

@Injectable()
export class CommonService {
    constructor(private _http: HttpClient) { }

    public getInstrumentDropdownInfo(projectId: string): Observable<InstrumentDropdownInfoDtoModel> {
        return this._http.get<InstrumentDropdownInfoDtoModel>(
            `${environment.apiUrl}Common/GetInstrumentsDropdownInfo?projectId=${projectId}`
        );
    }

    public getNonInstrumentDropdownInfo(projectId: string): Observable<NonInstrumentDropdownInfoDtoModel> {
        return this._http.get<NonInstrumentDropdownInfoDtoModel>(
            `${environment.apiUrl}Common/GetNonInstrumentsDropdownInfo?projectId=${projectId}`
        );
    }

    public getMemoryCacheItem(key: string): Observable<string[]> {
        return this._http.get<string[]>(
            `${environment.apiUrl}Common/GetMemoryCacheItem?key=${key}`
        );
    }

    public setMemoryCacheItem(model: AddMemoryCacheModel): Observable<void> {
        return this._http.post<void>(
            `${environment.apiUrl}Common/SetMemoryCache`,
            model
        );
    }
}