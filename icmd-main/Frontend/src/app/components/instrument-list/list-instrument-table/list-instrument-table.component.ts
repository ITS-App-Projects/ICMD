import { NgScrollbarModule } from 'ngx-scrollbar';
import {
  Subject,
  Subscription
} from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';

import { SelectionModel } from '@angular/cdk/collections';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  QueryList,
  ViewChild,
  ViewChildren
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import {
  MatPaginator,
  MatPaginatorModule
} from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import {
  MatSort,
  MatSortModule
} from '@angular/material/sort';
import {
  MatTable,
  MatTableDataSource,
  MatTableModule
} from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ColumnFilterComponent } from '@c/shared/column-filter';
import { FormDefaultsModule } from '@c/shared/forms';
import { NoRecordComponent } from '@c/shared/no-record';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchSortType } from '@e/search';
import {
  ActiveInActiveDtoModel,
  PagingDataModel,
  SortingDataModel
} from '@m/common';
import { instrumentListTableColumns } from '@u/constants';
import {
  defaultPageSize,
  pageSizeOptions
} from '@u/default';
import { FilterColumnsPipe } from '@u/pipe';

import { ViewInstrumentListLiveModel } from './list-instrument-table.model';

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
    @Output() public deleteBulk = new EventEmitter<any[]>();
    @Output() public activeInActive = new EventEmitter<ActiveInActiveDtoModel>();
    @Output() columnsChanged = new EventEmitter<void>();
    @Input() dataSource: MatTableDataSource<ViewInstrumentListLiveModel>;
    @Input() totalLength: number = 0;
    @Input() tagFieldNames: string[] = [];
    selection = new SelectionModel<ViewInstrumentListLiveModel>(true, []);
    selectedRowIds: Set<string> = new Set();

    public displayedColumns = [...instrumentListTableColumns].map(x => x.key);
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;
    protected defaultPageSize = defaultPageSize

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();

    showInstrument: boolean = false;
    private subscription!: Subscription;

    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService, private cdr: ChangeDetectorRef) { }

    @Input() public set items(value: ReadonlyArray<ViewInstrumentListLiveModel>) {
        this.dataSource = new MatTableDataSource([...value]);
        this.restoreSelection();
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
            this.restoreSelection();
        });
    }

    ngAfterViewChecked() {
        if (this.selection.selected.length > 0) {
            this.selection.selected.forEach(row => {
                if (!this.selectedRowIds.has(row.deviceId)) {
                    this.selectedRowIds.add(row.deviceId);
                }
            });
        }
    }

    protected deleteDevice(id: string) {
        this.delete.emit(id);
    }

    protected deleteBulkDevices(): void {
        const selectedDevices = Array.from(
            new Map(this.selection.selected.map(item => [item.deviceId, item])).values()
        );
        this.deleteBulk.emit(selectedDevices);
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

    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected === numRows;
    }

    toggleAllRows() {
        if (this.isAllSelected()) {
            this.selection.clear();
            this.selectedRowIds.clear();
        } else {
            this.selection.select(...this.dataSource.data);
            this.dataSource.data.forEach(row => {
                this.selectedRowIds.add(row.deviceId);
            });
        }
    }

    checkboxLabel(row?: ViewInstrumentListLiveModel): string {
        if (!row) {
          return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
        }
        return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.deviceId + 1}`;
    }

    cancelBulkDelete() {
        this.bulkDeleteService.cancelBulkDelete();
    }

    showDeleteBulk() {
        this.subscription = this.bulkDeleteService
        .getCheckboxState('instrument')
        .subscribe((show) => {
            this.showInstrument = show;
            this.resetCheckboxes();

            if (this.showInstrument) {
                this.pageSizeOptions = [100];
                this.displayedColumns = ['select', ...instrumentListTableColumns.map((x) => x.key)];
                this.cdr.detectChanges();

                if (this._paginator) {
                    this._paginator.pageSize = 100;

                    this._paginator.page.next({
                        pageIndex: 0,
                        pageSize: this._paginator.pageSize,
                        length: this._paginator.length
                    });
                }
            } else {
                this.pageSizeOptions = [10, 25, 50, 100];
                this.displayedColumns = instrumentListTableColumns.map((x) => x.key).filter(x => x !== 'select');
                this.cdr.detectChanges();

                if (this._paginator) {
                    this._paginator.pageSize = this.defaultPageSize;

                    this._paginator.page.next({
                        pageIndex: 0,
                        pageSize: this._paginator.pageSize,
                        length: this._paginator.length
                    });
                }
            }

            this.columnsChanged.emit();
        });
    }

    restoreSelection() {
        const rowsToSelect = this.dataSource.data.filter(row => this.selectedRowIds.has(row.deviceId));
        this.selection.select(...rowsToSelect);
    }

    resetCheckboxes(): void {
        if (!this.showInstrument) {
            this.selection.clear();
            this.selectedRowIds.clear();
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
        this.subscription.unsubscribe();

        this.bulkDeleteService.cancelBulkDelete();
    }
}


