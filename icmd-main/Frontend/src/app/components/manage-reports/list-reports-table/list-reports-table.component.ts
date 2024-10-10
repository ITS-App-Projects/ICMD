import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReportListDtoModel } from "@p/admin/manage-reports/list-reports-page";

@Component({
    standalone: true,
    selector: "app-list-reports-table",
    templateUrl: "./list-reports-table.component.html",
    imports: [
        CommonModule
    ],
    providers: [],
})
export class ListReportsTableComponent {
    reportList: ReportListDtoModel[] = [];
}