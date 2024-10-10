import { SearchFilterType, SearchType } from "@e/common";

export interface CustomFieldSearchModel {
    fieldValue: string;
    fieldName: string;
    searchType: SearchType | SearchFilterType;
    isColumnFilter: boolean;
}
