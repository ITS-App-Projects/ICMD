import { NgScrollbarModule } from 'ngx-scrollbar';
import {
  takeUntil,
  Subject
} from 'rxjs';

import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
  ViewChild
} from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import {
  MatPaginator,
  MatPaginatorModule
} from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { PagingDataModel } from '@m/common';
import { pageSizeOptions } from '@u/default';
import { CapitalizePipe } from '@u/pipe/capitalize.pipe';

import { ChangeLogResponceDtoModel } from './list-logs-table.model';

@Component({
    standalone: true,
    selector: "app-list-logs-table",
    templateUrl: "./list-logs-table.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatExpansionModule,
        NgScrollbarModule,
        MatPaginatorModule,
        CapitalizePipe
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