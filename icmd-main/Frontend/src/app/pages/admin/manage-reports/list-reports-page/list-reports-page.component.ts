import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ListReportsTableComponent } from "@c/manage-reports/list-reports-table";
import { ExcelHelper } from "@u/helper";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { ReportService } from "src/app/service/report";
import { ReportListDtoModel } from "./list-reports-page.model";

@Component({
    standalone: true,
    selector: "app-list-reports-page",
    templateUrl: "./list-reports-page.component.html",
    imports: [
        CommonModule,
        ListReportsTableComponent
    ],
    providers: [ReportService, ExcelHelper]
})
export class ListReportsPageComponent {
    @ViewChild(ListReportsTableComponent) reportListTable: ListReportsTableComponent;
    private _destroy$ = new Subject<void>();

    constructor(private _reportService: ReportService, private _cdr: ChangeDetectorRef) {
        this.getAllReports();
    }

    private getAllReports(): void {
        this._reportService.getReportList()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.reportListTable.reportList = res;
                this._cdr.detectChanges();
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}