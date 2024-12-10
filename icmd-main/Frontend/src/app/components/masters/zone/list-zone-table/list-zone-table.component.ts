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
import { ZoneInfoDtoModel } from "./list-zone-table.model";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { masterZoneListTableColumn } from "@u/constants";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { FilterColumnsPipe } from "@u/pipe";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatCheckbox, MatCheckboxModule } from "@angular/material/checkbox";
import { BulkDeleteService } from "src/app/service/instrument/bulkDelete/bulk-delete.service";
import { ToastrService } from "ngx-toastr";

@Component({
    standalone: true,
    selector: "app-list-zone-table",
    templateUrl: "./list-zone-table.component.html",
    imports: [
        FormDefaultsModule,
        MatCheckbox,
        MatCheckboxModule,
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
export class ListZoneTableComponent implements OnInit {
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public deleteBulk = new EventEmitter<any[]>();
    @Output() public edit = new EventEmitter<string>();
    @Input() dataSource: MatTableDataSource<ZoneInfoDtoModel>;
    @Input() totalLength: number = 0;

    public displayedColumns = [...masterZoneListTableColumn].map(x => x.key);
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();
    private _toastr: ToastrService;

    showZoneMaster: boolean = false;
    private subscription!: Subscription

    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService) { }

    @Input() public set items(value: ReadonlyArray<ZoneInfoDtoModel>) {
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

    protected deleteZone(id: string) {
        this.delete.emit(id);
    }

    protected deleteBulkZone() {
        const selectedZones = this.dataSource.data.filter((zone) => zone.checked);
        console.log(selectedZones);

        if(selectedZones.length === 0) {
            console.log("no items selected!");
            return
        }

        this.deleteBulk.emit(selectedZones);
    }

    protected editZone(id: string) {
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
        .getCheckboxState('zoneMaster')
        .subscribe((show) => {
            this.showZoneMaster = show;
            this.resetCheckboxes();

            if (this.showZoneMaster) {
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