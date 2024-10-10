import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { UnassociatedTagsDtoModel } from "./view-unassociated-tags-report.model";

@Component({
    standalone: true,
    selector: "app-view-unassociated-tags-report",
    templateUrl: "./view-unassociated-tags-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule
    ],
    providers: [],
})
export class ViewUnassociatedTagsReportComponent {
    protected tagsData: UnassociatedTagsDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: UnassociatedTagsDtoModel[]) {
        this.tagsData = val;
        this._cdr.detectChanges();
    }
}