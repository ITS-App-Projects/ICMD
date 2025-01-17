import { NgScrollbarModule } from 'ngx-scrollbar';

import { CommonModule } from '@angular/common';
import {
  Component,
  Input
} from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { NoRecordComponent } from '@c/shared/no-record';

import { ChangeLogResponceDtoModel } from './list-logs-table.model';

@Component({
    standalone: true,
    selector: "app-list-logs-table",
    templateUrl: "./list-logs-table.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        NoRecordComponent,
        MatExpansionModule,
        NgScrollbarModule,
    ],
    providers: []
})
export class ListLogsTableComponent {
    @Input() changeLogsData: ChangeLogResponceDtoModel[] = [];

    constructor() { }
}