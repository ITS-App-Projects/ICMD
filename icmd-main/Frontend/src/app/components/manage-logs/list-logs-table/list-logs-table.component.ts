import { NgScrollbarModule } from 'ngx-scrollbar';

import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  AfterViewInit,
  OnDestroy
} from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { NoRecordComponent } from '@c/shared/no-record';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';


import { ChangeLogResponceDtoModel } from './list-logs-table.model';
import { pageSizeOptions } from '@u/default';
import { PagingDataModel } from '@m/common';
import { Subject, takeUntil } from 'rxjs';

@Component({
    standalone: true,
    selector: "app-list-logs-table",
    templateUrl: "./list-logs-table.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatExpansionModule,
        NgScrollbarModule,
        MatPaginatorModule
    ],
    providers: []
})
export class ListLogsTableComponent implements AfterViewInit, OnDestroy {
    @Input() changeLogsData: ChangeLogResponceDtoModel[] = [];
    @Input() totalLength!: number;
    @Output() public pagingChanged = new EventEmitter<PagingDataModel>();

    protected pageSizeOptions = pageSizeOptions;
    @ViewChild(MatPaginator) private _paginator: MatPaginator;
    private _destroy$ = new Subject<void>();

    constructor() { }

    ngAfterViewInit() {
        this._paginator.page.pipe(takeUntil(this._destroy$)).subscribe((page) => {
            this.pagingChanged.emit({
                pageSize: page.pageSize,
                pageNumber: page.pageIndex + 1,
            });
        });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}