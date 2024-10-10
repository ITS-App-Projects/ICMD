import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewInstrumentListLiveDtoModel } from "../view-duplicate-dpnode-report";

@Component({
    standalone: true,
    selector: "app-view-instrument-list-report",
    templateUrl: "./view-instrument-list-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewInstrumentListReportComponent {
    protected instrumentList: ViewInstrumentListLiveDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewInstrumentListLiveDtoModel[]) {
        this.instrumentList = val;
        this._cdr.detectChanges();
    }
}