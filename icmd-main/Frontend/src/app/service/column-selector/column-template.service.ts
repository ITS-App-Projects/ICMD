import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@env/environment";
import { BaseDataResponseModel, BaseResponseModel } from "@m/auth/login-response-model";
import { CreateMetaDataModel, TemplateListModel } from "@p/dialog/column-selector/add-column-template-dialog";
import { Observable } from "rxjs";

@Injectable()
export class ColumnTemplateService {
    constructor(private _http: HttpClient) { }

    public getColumnTemplate(): Observable<TemplateListModel[]> {
        return this._http.get<TemplateListModel[]>(
            `${environment.apiUrl}Common/GetColumnTemplate`
        );
    }

    public createColumnTemplate(model: CreateMetaDataModel) {
        return this._http.post<BaseDataResponseModel<{ id: string }>>(
            `${environment.apiUrl}Common/CreateColumnTemplate`,
            model
        );
    }

    public deleteColumnTemplate(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Common/DeleteColumnTemplate?templateId=${id}`
        );
    }
}