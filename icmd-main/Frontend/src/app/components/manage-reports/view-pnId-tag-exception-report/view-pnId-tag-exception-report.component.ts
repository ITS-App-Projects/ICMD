import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { PnIDTagExceptionInfoDtoModel } from "./view-pnId-tag-exception-report.model";

@Component({
    standalone: true,
    selector: "app-view-pnId-tag-exception-report",
    templateUrl: "./view-pnId-tag-exception-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewPnIdTagExceptionReportComponent {
    protected pnIdTagData: PnIDTagExceptionInfoDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: PnIDTagExceptionInfoDtoModel[]) {
        this.pnIdTagData = val;
        this._cdr.detectChanges();
    }

    protected toggleCollapse(index: number): void {
        this.pnIdTagData[index].expanded = !this.pnIdTagData[index].expanded;
    }
}