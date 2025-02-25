import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ChangeLogResponceDtoModel } from "@c/manage-logs/list-logs-table";
import { environment } from "@env/environment";
import { PagedResultModel } from "@m/common";
import { UIChangeLogRequestDtoModel, UIChangeLogTypeDropdownInfoDtoModel } from "@p/admin/manage-logs/list-logs-page";
import { Observable } from "rxjs";

@Injectable()
export class LogsService {
    constructor(private _http: HttpClient) { }

    public getChangeLogTypes(projectId: string): Observable<UIChangeLogTypeDropdownInfoDtoModel> {
        return this._http.get<UIChangeLogTypeDropdownInfoDtoModel>(
            `${environment.apiUrl}UILogs/GetChangeLogTypes?projectId=${projectId}`
        );
    }

    public getChangeLogsData(request: UIChangeLogRequestDtoModel): Observable<PagedResultModel<ChangeLogResponceDtoModel>> {
        return this._http.post<PagedResultModel<ChangeLogResponceDtoModel>>(
            `${environment.apiUrl}UILogs/GetTypeWiseChangeLogs`, request
        );
    }
}