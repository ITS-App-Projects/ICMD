import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ViewInstrumentListLiveModel } from '@c/instrument-list/list-instrument-table';
import { environment } from '@env/environment';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

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

    public validateImportInstrument(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>> (
            `${environment.apiUrl}Instrument/ValidateImportInstruments`,
            formData
        );
    }

    public importInstruments(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>>(
            `${environment.apiUrl}Instrument/ImportInstruments`,
            formData
        );
    }
}