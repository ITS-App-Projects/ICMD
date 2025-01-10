import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatPaginator, MatPaginatorModule } from "@angular/material/paginator";
import { MatSort, MatSortModule } from "@angular/material/sort";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { FormsModule } from "@angular/forms";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { NoRecordComponent } from "@c/shared/no-record";
import { PagingDataModel, SortingDataModel } from "@m/common";
import { BankInfoDtoModel } from "./list-bank-table.model";
import { pageSizeOptions } from "@u/default";
import { Subject, Subscription } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { SearchSortType } from "@e/search";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { FilterColumnsPipe } from "@u/pipe";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { BulkDeleteService } from "src/app/service/instrument/bulkDelete/bulk-delete.service";

@Component({
    standalone: true,
    selector: "app-list-bank-table",
    templateUrl: "./list-bank-table.component.html",
    imports: [
        FormDefaultsModule,
        MatCheckboxModule,
        FormsModule,
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
export class ListBankTableComponent implements OnInit {
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public deleteBulk = new EventEmitter<any[]>();
    @Output() public edit = new EventEmitter<string>();
    @Input() dataSource: MatTableDataSource<BankInfoDtoModel>;
    @Input() totalLength: number = 0;

    protected displayedColumns = [
        "bank",
        "actions",
    ];
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();


    showBankMaster: boolean = false;
    private subscription!: Subscription;
    
    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService) { }

    @Input() public set items(value: ReadonlyArray<BankInfoDtoModel>) {
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

    protected deleteBank(id: string) {
        this.delete.emit(id);
    }

    protected editBank(id: string) {
        this.edit.emit(id);
    }

    protected applyFilter(search: string) {
        this.search.emit(search);
    }

    protected deleteBulkBank(): void {
        const selectedDevices = this.dataSource.data.filter((element) => element.checked);
        this.deleteBulk.emit(selectedDevices);
    }

    cancelBulkDelete() {
        this.bulkDeleteService.cancelBulkDelete();
    }

    showDeleteBulk() {
        this.subscription = this.bulkDeleteService
        .getCheckboxState('bankMaster')
        .subscribe((show) => {
            this.showBankMaster = show;
            this.resetCheckboxes();

            if (this.showBankMaster) {
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