import { Component, EventEmitter, Input, OnChanges, Output, QueryList, SimpleChanges, ViewChild, ViewChildren, OnInit, OnDestroy } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatPaginator, MatPaginatorModule } from "@angular/material/paginator";
import { MatSort, MatSortModule } from "@angular/material/sort";
import { MatTable, MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { NoRecordComponent } from "@c/shared/no-record";
import { ActiveInActiveDtoModel, PagingDataModel, SortingDataModel } from "@m/common";
import { pageSizeOptions } from "@u/default";
import { Subject } from "rxjs";
import { Subscription } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { SearchSortType } from "@e/search";
import { ViewInstrumentListLiveModel } from "./list-instrument-table.model";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { instrumentListTableColumns } from "@u/constants";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { FilterColumnsPipe } from "@u/pipe";
import { MatTooltipModule } from "@angular/material/tooltip";
import { NgScrollbarModule } from "ngx-scrollbar";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import {MatCheckboxModule} from '@angular/material/checkbox';
import { BulkDeleteService } from "@c/shared/list-actions/bulk-delete.service";

@Component({
    standalone: true,
    selector: "app-list-instrument-table",
    templateUrl: "./list-instrument-table.component.html",
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
        MatTooltipModule,
        MatProgressBarModule,
        NgScrollbarModule,
        MatCheckboxModule
    ],
    providers: [],
})
export class ListInstrumentTableComponent implements OnInit, OnDestroy {
    @ViewChild('table') table: MatTable<any>;
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public edit = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public deleteBulk = new EventEmitter<string>()
    @Output() public activeInActive = new EventEmitter<ActiveInActiveDtoModel>();
    @Input() dataSource: MatTableDataSource<ViewInstrumentListLiveModel>;
    @Input() totalLength: number = 0;
    @Input() tagFieldNames: string[] = [];

    public displayedColumns = [...instrumentListTableColumns].map(x => x.key);
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();

    showCheckboxes: boolean = false;
    private subscription!: Subscription;
    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService) { }

    @Input() public set items(value: ReadonlyArray<ViewInstrumentListLiveModel>) {
        this.dataSource = new MatTableDataSource([...value]);
    }

    ngOnInit(): void {
        this.subscription = this.bulkDeleteService.showCheckboxes$.subscribe(
            (show) => (this.showCheckboxes = show)
          );
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

    protected deleteDevice(id: string) {
        this.delete.emit(id);
    }

    protected deleteBulkDevices(ids: string[]) {

    }

    protected editInstrument(id: string) {
        this.edit.emit(id);
    }

    protected applyFilter(search: string) {
        this.search.emit(search);
    }

    protected activeInActiveStatus(id: string, isActive: boolean) {
        const info: ActiveInActiveDtoModel = {
            id: id,
            isActive: isActive
        };
        this.activeInActive.emit(info);
    }


    cancelBulkDelete() {
        this.bulkDeleteService.toggleCheckboxes(false);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
        this.subscription.unsubscribe();
    }
}

//#region Reserved Comments 


// getSelectedIds() {
//     return this.data.filter((row) => row.checked).map((row) => row.id);
// }

// deleteAllSelected() {
//     const selectedIds = this.getSelectedIds();
//     if (selectedIds.length > 0) {
//       this.delete.emit(selectedIds.join(',')); 
//     } else {
//       alert('No rows selected for deletion.');
//     }
// }


//#endregion