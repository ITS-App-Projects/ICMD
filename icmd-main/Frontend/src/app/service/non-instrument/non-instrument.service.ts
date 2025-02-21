import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ViewNonInstrumentListDtoModel } from '@c/nonInstrument-list/list-nonInstrument-table';
import { environment } from '@env/environment';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

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

    public validateImportNonInstrument(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>> (
            `${environment.apiUrl}NonInstrument/ValidateImportNonInstruments`,
            formData
        );
    }

    public importNonInstruments(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>>(
            `${environment.apiUrl}NonInstrument/ImportNonInstruments`,
            formData
        );
    }
}