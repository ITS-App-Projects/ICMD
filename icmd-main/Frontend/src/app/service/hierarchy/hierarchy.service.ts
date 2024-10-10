import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { HierarchyRequestDtoModel, HierarchyResponceDtoModel } from "@c/hierarchy/list-hierarchy-table";
import { environment } from "@env/environment";
import { Observable } from "rxjs";

@Injectable()
export class HierarchyService {
    constructor(private _http: HttpClient) { }

    public getHierarchyData(
        request: HierarchyRequestDtoModel
    ): Observable<HierarchyResponceDtoModel> {
        return this._http.post<HierarchyResponceDtoModel>(
            `${environment.apiUrl}Hierarchy/GetHierarchyData`,
            request
        );
    }
}