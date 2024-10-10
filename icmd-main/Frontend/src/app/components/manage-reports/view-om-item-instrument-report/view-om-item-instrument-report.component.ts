import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewOMItemInstrumentListDtoModel } from "./view-om-item-instrument-report.model";

@Component({
    standalone: true,
    selector: "app-view-om-item-instrument-report",
    templateUrl: "./view-om-item-instrument-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewOMItemInstrumentReportComponent {
    protected omInstrumentList: ViewOMItemInstrumentListDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewOMItemInstrumentListDtoModel[]) {
        this.omInstrumentList = val;
        this._cdr.detectChanges();
    }
}