import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { SparesReportResponceDtoModel } from "./view-spares-report.model";

@Component({
    standalone: true,
    selector: "app-view-spares-report",
    templateUrl: "./view-spares-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewSparesReportComponent {
    protected sparesData: SparesReportResponceDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: SparesReportResponceDtoModel[]) {
        this.sparesData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(type: string, plcIndex: number, rackIndex: number): void {
        if (type == "PLCNumber")
            this.sparesData[plcIndex].expanded = !this.sparesData[plcIndex].expanded;

        if (type == "Rack")
            this.sparesData[plcIndex].childItems[rackIndex].expanded = !this.sparesData[plcIndex].childItems[rackIndex].expanded;
    }
}