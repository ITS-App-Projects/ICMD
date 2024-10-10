import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { DuplicateDPNodeAddressDtoModel } from "./view-duplicate-dpnode-report.model";

@Component({
    standalone: true,
    selector: "app-view-duplicate-dpnode-report",
    templateUrl: "./view-duplicate-dpnode-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewDuplicateDPNodeReportComponent {
    protected nodeData: DuplicateDPNodeAddressDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: DuplicateDPNodeAddressDtoModel[]) {
        this.nodeData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(index: number): void {
        this.nodeData[index].expanded = !this.nodeData[index].expanded;
    }
}