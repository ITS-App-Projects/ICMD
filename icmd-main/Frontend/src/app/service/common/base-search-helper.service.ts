import { SearchSortType } from "@e/search";
import { CustomFieldSearchModel, PagedAndSortedResultRequestModel, PagedResultModel, PagingDataModel, SortingDataModel } from "@m/common";
import { dashboardDefaultPageSize, defaultPageSize } from "@u/default";
import { BehaviorSubject, Observable } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";

export abstract class BaseSearchHelperService<TSearchObjectModel> {
    public searchData$: BehaviorSubject<ReadonlyArray<TSearchObjectModel>> =
        new BehaviorSubject([]);
    protected searchRequestModel$: BehaviorSubject<PagedAndSortedResultRequestModel> =
        new BehaviorSubject(null);
    public totalCount$: BehaviorSubject<number> = new BehaviorSubject(0);

    defaultSearchRequestModel: PagedAndSortedResultRequestModel = {
        sorting: "",
        sortAcending: false,
        pageNumber: 1,
        pageSize: defaultPageSize,
        search: "",
        projectId: null,
        customSearchList: [], //CustomFieldSearchModel[];
    };
    protected constructor() { }

    public loadDataFromRequest(
        isDashboard: boolean = false
    ): Observable<PagedResultModel<TSearchObjectModel>> {
        return this.searchRequestModel$.pipe(
            map((value) => {
                if (!value) {
                    if (isDashboard)
                        this.defaultSearchRequestModel.pageSize = dashboardDefaultPageSize;
                    else
                        this.defaultSearchRequestModel.pageSize = defaultPageSize;

                    return this.defaultSearchRequestModel;
                }
                return value;
            }),
            switchMap((request) => this.search(request)),
            tap((response) => this.totalCount$.next(response.totalCount ?? 0)),
            tap((response) => this.searchData$.next(this.getItems(response)))
        );
    }

    public updatePagingChange(model: PagingDataModel): void {
        this.searchRequestModel$.next({
            ...this.defaultSearchRequestModel,
            ...this.searchRequestModel$.value,
            pageNumber: model.pageNumber,
            pageSize: model.pageSize,
        });
    }

    public updateFilterChange(model: CustomFieldSearchModel[]): void {
        this.searchRequestModel$.next({
            ...this.defaultSearchRequestModel,
            ...this.searchRequestModel$.value,
            customSearchList: model,
        });
    }

    public commonSearch(search: string): void {
        this.searchRequestModel$.next({
            ...this.defaultSearchRequestModel,
            ...this.searchRequestModel$.value,
            search: search,
        });
    }

    public updateSortingChange(model: SortingDataModel): void {
        this.searchRequestModel$.next({
            ...this.defaultSearchRequestModel,
            ...this.searchRequestModel$.value,
            sorting: model.sortField,
            sortAcending: model.sortType == SearchSortType.Ascending ? true : false,
        });
    }

    public updateProjectId(projectId: string) {
        this.searchRequestModel$.next({
            ...this.defaultSearchRequestModel,
            ...this.searchRequestModel$.value,
            projectId: projectId,
        });
    }

    protected abstract search(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TSearchObjectModel>>;

    protected abstract getItems(
        response: PagedResultModel<TSearchObjectModel>
    ): ReadonlyArray<TSearchObjectModel>;
}
