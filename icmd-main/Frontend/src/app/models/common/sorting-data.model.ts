import { SearchSortType } from '@e/search';

export interface SortingDataModel {
    readonly sortField: string;
    readonly sortType: SearchSortType;
}
