import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewNonInstrumentListDtoModel } from "@c/nonInstrument-list/list-nonInstrument-table";

@Component({
    standalone: true,
    selector: "app-view-non-instrument-list-report",
    templateUrl: "./view-non-instrument-list-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewNonInstrumentListReportComponent {
    protected nonInstrumentList: ViewNonInstrumentListDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewNonInstrumentListDtoModel[]) {
        this.nonInstrumentList = val;
        this._cdr.detectChanges();
    }
}