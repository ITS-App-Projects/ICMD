import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { TypeInfoDtoModel } from "@c/masters/documentType/list-document-type-table";
import { DocumentTypeService } from "./documentType.service";

@Injectable()
export class DocumentTypeSearchHelperService extends BaseSearchHelperService<TypeInfoDtoModel> {
    constructor(private _documentTypeService: DocumentTypeService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TypeInfoDtoModel>> {
        return this._documentTypeService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<TypeInfoDtoModel>
    ): ReadonlyArray<TypeInfoDtoModel> {
        return response.items ?? [];
    }
}