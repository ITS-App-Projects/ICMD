import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { DuplicateRackSlotChannelDtoModel } from "./view-duplicate-rack-slot-report.model";

@Component({
    standalone: true,
    selector: "app-view-duplicate-rack-slot-report",
    templateUrl: "./view-duplicate-rack-slot-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewDuplicateRackSlotReportComponent {
    protected nodeData: DuplicateRackSlotChannelDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: DuplicateRackSlotChannelDtoModel[]) {
        this.nodeData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(index: number): void {
        this.nodeData[index].expanded = !this.nodeData[index].expanded;
    }
}