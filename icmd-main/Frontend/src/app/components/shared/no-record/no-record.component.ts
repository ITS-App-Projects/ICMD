/* eslint-disable @typescript-eslint/member-ordering */
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
    standalone: true,
    selector: 'app-no-record',
    templateUrl: './no-record.component.html',
    imports: [CommonModule]
})
export class NoRecordComponent {
    @Input() noRecordLabel: string = '';
    @Input() isCenter: boolean = true;
}
