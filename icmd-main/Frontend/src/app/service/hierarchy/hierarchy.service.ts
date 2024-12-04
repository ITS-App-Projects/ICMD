import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  HierarchyRequestDtoModel,
  HierarchyResponceDtoModel
} from '@c/hierarchy/list-hierarchy-table';
import {
  ChildrenRequestDtoModel,
  HierarchyDeviceInfoDtoModel
} from '@c/hierarchy/list-hierarchy-table/list-hierarchy-table.model';
import { environment } from '@env/environment';

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
    ): Observable<HierarchyDeviceInfoDtoModel[]> {
        return this._http.post<HierarchyDeviceInfoDtoModel[]>(
            `${environment.apiUrl}Hierarchy/GetHierarchyChilds`,
            request
        );
    }
}