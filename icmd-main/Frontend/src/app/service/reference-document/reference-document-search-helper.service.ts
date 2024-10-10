import { Injectable } from "@angular/core";
import { BaseSearchHelperService } from "../common";
import { PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";
import { ReferenceDocumentInfoDtoModel } from "@c/masters/reference-document/list-reference-document-table";
import { ReferenceDocumentService } from "./reference-document.service";

@Injectable()
export class ReferenceDocumentSearchHelperService extends BaseSearchHelperService<ReferenceDocumentInfoDtoModel> {
    constructor(private _referenceDocumentService: ReferenceDocumentService) {
        super();
    }

    protected search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ReferenceDocumentInfoDtoModel>> {
        return this._referenceDocumentService.getAll(request);
    }

    protected getItems(
        response: PagedResultModel<ReferenceDocumentInfoDtoModel>
    ): ReadonlyArray<ReferenceDocumentInfoDtoModel> {
        return response.items ?? [];
    }
}