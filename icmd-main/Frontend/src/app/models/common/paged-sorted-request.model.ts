import { CustomFieldSearchModel } from ".";

export interface PagedAndSortedResultRequestModel {
    sorting: string;
    sortAcending: boolean;
    pageNumber: number;
    pageSize: number;
    search: string;
    projectId: string | null;
    customSearchList: Array<CustomFieldSearchModel>;
}
