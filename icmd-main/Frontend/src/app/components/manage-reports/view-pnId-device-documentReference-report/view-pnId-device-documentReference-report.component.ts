import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewPnIDDeviceDocumentReferenceCompareDtoModel } from "./view-pnId-device-documentReference-report.model";

@Component({
    standalone: true,
    selector: "app-view-pnId-device-documentReference-report",
    templateUrl: "./view-pnId-device-documentReference-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewPnIdDeviceDocumentReferenceReportComponent {
    protected tagData: ViewPnIDDeviceDocumentReferenceCompareDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewPnIDDeviceDocumentReferenceCompareDtoModel[]) {
        this.tagData = val;
        this._cdr.detectChanges();
    }
}