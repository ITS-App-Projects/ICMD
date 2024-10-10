import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { ChangeLogListDtoModel } from "./view-auditLog-report.model";
import { MatTableModule } from "@angular/material/table";
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-view-auditLog-report",
    templateUrl: "./view-auditLog-report.component.html",
    imports: [
        CommonModule,
        MatTableModule,
        MatInputModule,
        MatIconModule,
        NgScrollbarModule
    ],
    providers: [],
})
export class ViewAuditLogReportComponent {
    protected auditLogData: ChangeLogListDtoModel[] = [];
    private searchAuditLogData: ChangeLogListDtoModel[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set item(val: ChangeLogListDtoModel[]) {
        this.auditLogData = val;
        this.searchAuditLogData = val;
        this._cdr.detectChanges();
    }

    protected searchAuditLog(event: string): void {
        this.auditLogData = this.searchAuditLogData;
        if (event) {
            this.auditLogData = this.auditLogData.filter(log => {
                return log.items && log.items.some(item => (
                    (item.context?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.entityName?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.contextId?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.status?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.originalValues?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.newValues?.toLowerCase().includes(event?.toLowerCase())) ||
                    (item.createdBy?.toLowerCase().includes(event?.toLowerCase()))
                ));
            });
            this._cdr.detectChanges();
        }
    }

    protected toggleCollapse(index: number): void {
        this.auditLogData[index].expanded = !this.auditLogData[index].expanded;
    }

}