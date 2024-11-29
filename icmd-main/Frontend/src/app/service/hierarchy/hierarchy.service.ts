import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { HierarchyRequestDtoModel, HierarchyResponceDtoModel } from "@c/hierarchy/list-hierarchy-table";
import { ChildrenRequestDtoModel, ChildrenResponseDtoModel } from "@c/hierarchy/list-hierarchy-table/list-hierarchy-table.model";
import { environment } from "@env/environment";
import { Observable } from "rxjs";

@Injectable()
export class HierarchyService {
    constructor(private _http: HttpClient) { }

    public getParentsData(
        request: HierarchyRequestDtoModel
    ): Observable<HierarchyResponceDtoModel> {
        return this._http.post<HierarchyResponceDtoModel>(
            `${environment.apiUrl}Hierarchy/GetHierarchyData`,
            request
        );
    }

    public getChildrenData(
        request: ChildrenRequestDtoModel
    ): Observable<ChildrenResponseDtoModel> {
        return this._http.post<ChildrenResponseDtoModel>(
            `${environment.apiUrl}Hierarchy/GetHierarchyChilds`,
            request
        );
    }
}