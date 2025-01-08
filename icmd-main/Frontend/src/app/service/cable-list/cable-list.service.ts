import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ViewCableListDtoModel } from '@c/cableList/list-cableList-table';
import { environment } from '@env/environment';
import {
  ImportFileResultModel,
  PagedAndSortedResultRequestModel,
  PagedResultModel
} from '@m/common';

@Injectable()
export class CableListService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ViewCableListDtoModel>> {
        return this._http.post<PagedResultModel<ViewCableListDtoModel>>(
            `${environment.apiUrl}NonInstrument/GetAllNonInstruments`,
            request
        );
    }

    public importCable(projectId: string, file: File): Observable<ImportFileResultModel<[]>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<[]>>(
            `${environment.apiUrl}NonInstrument/ImportNonInstruments`,
            formData
        );
    }
}