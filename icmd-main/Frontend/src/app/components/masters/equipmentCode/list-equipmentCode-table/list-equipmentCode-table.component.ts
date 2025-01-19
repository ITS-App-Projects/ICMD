import {
  Subject,
  Subscription
} from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';

import { SelectionModel } from '@angular/cdk/collections';
import {
  inject,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  QueryList,
  ViewChild,
  ViewChildren
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckbox } from '@angular/material/checkbox';
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
  MatTableDataSource,
  MatTableModule
} from '@angular/material/table';
import { ColumnFilterComponent } from '@c/shared/column-filter';
import { FormDefaultsModule } from '@c/shared/forms';
import { NoRecordComponent } from '@c/shared/no-record';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchSortType } from '@e/search';
import {
  PagingDataModel,
  SortingDataModel
} from '@m/common';
import { masterEquipmentCodeListTableColumn } from '@u/constants';
import { pageSizeOptions } from '@u/default';
import { FilterColumnsPipe } from '@u/pipe';

import { EquipmentCodeInfoDtoModel } from './list-equipmentCode-table.model';

@Component({
    standalone: true,
    selector: "app-list-equipmentCode-table",
    templateUrl: "./list-equipmentCode-table.component.html",
    imports: [
        FormDefaultsModule,
        MatTableModule,
        MatCheckbox,
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
export class ListEquipmentCodeTableComponent implements OnInit {
    @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
    @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
    @Output() public search = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    @Output() public deleteBulk = new EventEmitter<any[]>();
    @Output() public edit = new EventEmitter<string>();
    @Output() columnsChanged = new EventEmitter<void>();
    @Input() dataSource: MatTableDataSource<EquipmentCodeInfoDtoModel>;
    @Input() totalLength: number = 0;
    selection = new SelectionModel<EquipmentCodeInfoDtoModel>(true, []);
    selectedRowIds: Set<string> = new Set();

    public displayedColumns = [...masterEquipmentCodeListTableColumn].map(x => x.key);
    protected isLoading: boolean;
    protected pageSizeOptions = pageSizeOptions;

    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    @ViewChild(MatSort) private _sort: MatSort;
    private _destroy$ = new Subject<void>();
    private cdr = inject(ChangeDetectorRef)

    showEquipmentCode: boolean = false;
    private subscription: Subscription;

    constructor(protected appConfig: AppConfig, private bulkDeleteService: BulkDeleteService) { }

    @Input() public set items(value: ReadonlyArray<EquipmentCodeInfoDtoModel>) {
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
                if (!this.selectedRowIds.has(row.id)) {
                    this.selectedRowIds.add(row.id);
                }
            });
        }
    }

    protected deleteEquipmentCode(id: string) {
        this.delete.emit(id);
    }

    protected deleteBulkEquipmentCode() {
        const selected = Array.from(
            new Map(this.selection.selected.map(item => [item.id, item])).values()
        );
        this.deleteBulk.emit(selected);
    }

    protected editEquipmentCode(id: string) {
        this.edit.emit(id);
    }

    protected applyFilter(search: string) {
        this.search.emit(search);
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
                this.selectedRowIds.add(row.id);
            });
        }
    }

    checkboxLabel(row?: EquipmentCodeInfoDtoModel): string {
        if (!row) {
          return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
        }
        return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.id + 1}`;
    }

    cancelBulkDelete() {
        this.bulkDeleteService.cancelBulkDelete();
    }

    showDeleteBulk() {
        this.subscription = this.bulkDeleteService
        .getCheckboxState('equipmentCode')
        .subscribe((show) => {
            this.showEquipmentCode = show;
            this.resetCheckboxes();

            if (this.showEquipmentCode) {
                this.pageSizeOptions = [100];
                this.displayedColumns = ['select', ...masterEquipmentCodeListTableColumn.map((x) => x.key)];
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
                this.displayedColumns = masterEquipmentCodeListTableColumn.map((x) => x.key).filter(x => x != 'select');
                this.cdr.detectChanges();

                if (this._paginator) {
                    this._paginator.pageSize = pageSizeOptions[0];

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
        const rowsToSelect = this.dataSource.data.filter(row => this.selectedRowIds.has(row.id));
        this.selection.select(...rowsToSelect);
    }

    resetCheckboxes(): void {
        if (!this.showEquipmentCode) {
            this.selection.clear();
            this.selectedRowIds.clear();
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();

        this.bulkDeleteService.cancelBulkDelete();
    }
}