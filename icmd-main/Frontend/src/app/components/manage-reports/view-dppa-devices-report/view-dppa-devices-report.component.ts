import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { DPPADevicesDtoModel } from "./view-dppa-devices-report.model";

@Component({
    standalone: true,
    selector: "app-view-dppa-devices-report",
    templateUrl: "./view-dppa-devices-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewDPPADevicesReportComponent {
    protected devicesData: DPPADevicesDtoModel[] = [];
    
    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: DPPADevicesDtoModel[]) {
        this.devicesData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(type: string, plcIndex: number, plcSlotIndex: number, couplerIndex: number): void {
        if (type == "PLCNumber")
            this.devicesData[plcIndex].expanded = !this.devicesData[plcIndex].expanded;

        if (type == "PLCSlotNumber") {
            this.devicesData[plcIndex].childInfo[plcSlotIndex].expanded = !this.devicesData[plcIndex].childInfo[plcSlotIndex].expanded;
        }

        if (type == "DPPACoupler") {
            this.devicesData[plcIndex].childInfo[plcSlotIndex].childInfo[couplerIndex].expanded = !this.devicesData[plcIndex].childInfo[plcSlotIndex].childInfo[couplerIndex].expanded;
        }

    }
}