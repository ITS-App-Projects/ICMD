import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewUnassociatedSkidsDto } from "./view-unassociated-skids-report.model";

@Component({
    standalone: true,
    selector: "app-view-unassociated-skids-report",
    templateUrl: "./view-unassociated-skids-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewUnassociatedSkidsReportComponent {
    isStand: boolean = false;
    protected listData: ViewUnassociatedSkidsDto[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewUnassociatedSkidsDto[]) {
        this.listData = val;
        this._cdr.detectChanges();
    }
}