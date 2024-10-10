import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { SparesReportDetailsResponceDtoModel } from "./view-spares-detail-report.model";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";

@Component({
    standalone: true,
    selector: "app-view-spares-detail-report",
    templateUrl: "./view-spares-detail-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewSparesDetailReportComponent {
    protected sparesData: SparesReportDetailsResponceDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: SparesReportDetailsResponceDtoModel[]) {
        this.sparesData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(type: string, plcIndex: number, rackIndex: number, natureIndex: number): void {
        if (type == "PLCNumber")
            this.sparesData[plcIndex].expanded = !this.sparesData[plcIndex].expanded;

        if (type == "Rack")
            this.sparesData[plcIndex].childItems[rackIndex].expanded = !this.sparesData[plcIndex].childItems[rackIndex].expanded;

        if (type == "NatureOfSignal")
            this.sparesData[plcIndex].childItems[rackIndex].childItems[natureIndex].expanded =
                !this.sparesData[plcIndex].childItems[rackIndex].childItems[natureIndex].expanded;
    }
}