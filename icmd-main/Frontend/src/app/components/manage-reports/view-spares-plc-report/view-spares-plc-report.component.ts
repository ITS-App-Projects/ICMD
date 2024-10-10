import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { SparesReportPLCResponceDtoModel } from "./view-spares-plc-report.model";

@Component({
    standalone: true,
    selector: "app-view-spares-plc-report",
    templateUrl: "./view-spares-plc-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewSparesPLCReportComponent {
    protected sparesData: SparesReportPLCResponceDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: SparesReportPLCResponceDtoModel[]) {
        this.sparesData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(type: string, plcIndex: number): void {
        if (type == "PLCNumber")
            this.sparesData[plcIndex].expanded = !this.sparesData[plcIndex].expanded;
    }
}