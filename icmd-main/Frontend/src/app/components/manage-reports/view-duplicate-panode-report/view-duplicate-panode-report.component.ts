import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { DuplicatePANodeAddressDtoModel } from "./view-duplicate-panode-report.model";

@Component({
    standalone: true,
    selector: "app-view-duplicate-panode-report",
    templateUrl: "./view-duplicate-panode-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewDuplicatePANodeReportComponent {
    protected nodeData: DuplicatePANodeAddressDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: DuplicatePANodeAddressDtoModel[]) {
        this.nodeData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(index: number): void {
        this.nodeData[index].expanded = !this.nodeData[index].expanded;
    }
}