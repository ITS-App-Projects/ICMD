import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from "@angular/core";
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
import { Subject, Subscription } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { SearchSortType } from "@e/search";
import { SystemInfoDtoModel } from "./list-system-table.model";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { masterSystemListTableColumn } from "@u/constants";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { FilterColumnsPipe } from "@u/pipe";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { BulkDeleteService } from "src/app/service/instrument/bulkDelete/bulk-delete.service";
import { MatCheckbox } from "@angular/material/checkbox";

@Component({
    standalone: true,
    selector: "app-list-system-table",
    templateUrl: "./list-system-table.component.html",
    imports: [
        FormDefaultsModule,
        MatTableModule,
        MatSortModule,
        MatCheckbox,
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
export class ListSystemTableComponent implements OnInit {
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public deleteBulk = new EventEmitter<any[]>();
    @Output() public edit = new EventEmitter<string>();
    @Input() dataSource: MatTableDataSource<SystemInfoDtoModel>;
    @Input() totalLength: number = 0;

    public displayedColumns = [...masterSystemListTableColumn].map(x => x.key);
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();

    showSystemMaster: boolean = false;
    private subscription: Subscription;

    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService) { }

    @Input() public set items(value: ReadonlyArray<SystemInfoDtoModel>) {
        this.dataSource = new MatTableDataSource([...value]);
    }

    ngOnInit(): void {
        this.showDeleteBulk();
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

    protected deleteSystem(id: string) {
        this.delete.emit(id);
    }

    protected deleteBulkSystem() {
        const selectedSystem = this.dataSource.data.filter((system) => system.checked);
        console.log(selectedSystem);
        
        this.deleteBulk.emit(selectedSystem);
    }

    protected editSystem(id: string) {
        this.edit.emit(id);
    }

    protected applyFilter(search: string) {
        this.search.emit(search);
    }

    cancelBulkDelete() {
        this.bulkDeleteService.cancelBulkDelete();
    }

    showDeleteBulk() {
        this.subscription = this.bulkDeleteService
        .getCheckboxState('systemMaster')
        .subscribe((show) => {
            this.showSystemMaster = show;
            this.resetCheckboxes();

            if (this.showSystemMaster) {
                this.pageSizeOptions = [100]; 
                if (this._paginator) {
                    this._paginator.pageSize = 100; 
                    this._paginator.pageIndex = 0; 
                    this.updateTable();
                }
            } else {
                this.pageSizeOptions = [10, 25, 50, 100]; 
                if (this._paginator) {
                    this._paginator.pageSize = this.pageSizeOptions[0]; 
                    this.updateTable();
                }
            }
        });
    }

    resetCheckboxes(): void {
        this.dataSource.data
       .forEach((item) => {
        item.checked = false;
       });
    }

    updateTable() {
        if (this.dataSource) {
            this.dataSource.paginator = this._paginator; 
            this.dataSource.data = [...this.dataSource.data]; 
        }

        this._paginator._changePageSize(this._paginator.pageSize);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();

        this.bulkDeleteService.cancelBulkDelete();
    }
}