import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewNatureOfSignalValidationFailuresDtoModel } from "./view-nature-of-signal-validations-report.model";

@Component({
    standalone: true,
    selector: "app-view-nature-of-signal-validations-report",
    templateUrl: "./view-nature-of-signal-validations-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewNatureOfSignalValidationsReportComponent {
    protected natureList: ViewNatureOfSignalValidationFailuresDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewNatureOfSignalValidationFailuresDtoModel[]) {
        this.natureList = val;
        this._cdr.detectChanges();
    }
}