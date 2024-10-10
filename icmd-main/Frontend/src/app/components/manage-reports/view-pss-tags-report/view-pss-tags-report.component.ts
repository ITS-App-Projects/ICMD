import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { ViewPSSTagsDtoModel } from "./view-pss-tags-report.model";

@Component({
    standalone: true,
    selector: "app-view-pss-tags-report",
    templateUrl: "./view-pss-tags-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewPSSTagsReportComponent {
    protected tagList: ViewPSSTagsDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ViewPSSTagsDtoModel[]) {
        this.tagList = val;
        this._cdr.detectChanges();
    }
}