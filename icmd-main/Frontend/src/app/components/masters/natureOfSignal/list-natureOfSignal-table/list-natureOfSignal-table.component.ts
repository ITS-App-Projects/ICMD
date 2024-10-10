import { Component, EventEmitter, Input, Output, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatPaginator, MatPaginatorModule } from "@angular/material/paginator";
import { MatSort, MatSortModule } from "@angular/material/sort";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { NoRecordComponent } from "@c/shared/no-record";
import { PagingDataModel, SortingDataModel } from "@m/common";
import { pageSizeOptions } from "@u/default";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { SearchSortType } from "@e/search";
import { NatureOfSignalListDtoModel } from "./list-natureOfSignal-table.model";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { FilterColumnsPipe } from "@u/pipe";
import { MatProgressBarModule } from "@angular/material/progress-bar";

@Component({
    standalone: true,
    selector: "app-list-natureOfSignal-table",
    templateUrl: "./list-natureOfSignal-table.component.html",
    imports: [
        FormDefaultsModule,
        MatTableModule,
        MatSortModule,
        NoRecordComponent,
        MatPaginatorModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        PermissionWrapperComponent,
        ColumnFilterComponent,
        FilterColumnsPipe,
        MatProgressBarModule
    ],
    providers: [],
})
export class ListNatureOfSignalTableComponent {
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public edit = new EventEmitter<string>();
    @Input() dataSource: MatTableDataSource<NatureOfSignalListDtoModel>;
    @Input() totalLength: number = 0;

    protected displayedColumns = [
        "natureOfSignalName",
        "actions",
    ];
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();

    constructor(protected appConfig: AppConfig) { }

    @Input() public set items(value: ReadonlyArray<NatureOfSignalListDtoModel>) {
        this.dataSource = new MatTableDataSource([...value]);
    }

    ngAfterViewInit() {
        this._sort.sortChange.pipe(takeUntil(this._destroy$)).subscribe((sort) => {
            this._paginator.firstPage();
            this.sortingChanged.emit({
                sortType:
                    sort.direction === "asc"
                        ? SearchSortType.Ascending
                        : SearchSortType.Descending,
                sortField: sort.active,
            });
        });

        this._paginator.page.pipe(takeUntil(this._destroy$)).subscribe((page) => {
            this.pagingChanged.emit({
                pageSize: page.pageSize,
                pageNumber: page.pageIndex + 1,
            });
        });
    }

    protected deleteNatureSignal(id: string) {
        this.delete.emit(id);
    }

    protected editNatureSignal(id: string) {
        this.edit.emit(id);
    }

    protected applyFilter(search: string) {
        this.search.emit(search);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}