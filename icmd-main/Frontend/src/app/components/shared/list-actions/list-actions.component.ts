import { CommonModule } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { Subject, takeUntil } from "rxjs";
import { PermissionWrapperComponent } from "../permission-wrapper";
import { AppConfig } from "src/app/app.config";
import { MatDividerModule } from "@angular/material/divider";
import { BulkDeleteService } from "./bulk-delete.service";

@Component({
    standalone: true,
    selector: "list-actions",
    templateUrl: "./list-actions.component.html",
    styleUrl: "./styles/list-actions.component.scss",
    imports: [CommonModule, MatIconModule, MatButtonModule, MatDividerModule, MatMenuModule],
})
export class ListActionsComponent implements OnDestroy, AfterViewInit, OnInit {
    @Input() showColunmSelector: boolean = true;
    @Input() showExport: boolean;
    @Output() isExport = new EventEmitter<boolean>(false);
    @Output() isColumnSelector = new EventEmitter<boolean>(false);
    @Output() isImport = new EventEmitter<boolean>(false);
    @Output() isImportFileDownload = new EventEmitter<boolean>(false);

    protected hasPermissionToImport: boolean = true;
    protected hasPermissionToExport: boolean = true;
    private _destroy$: Subject<void> = new Subject<void>();

    constructor(private appConfig: AppConfig, private cd: ChangeDetectorRef, private bulkDeleteService: BulkDeleteService) { }

    ngOnInit() {
        const permissionWrapperForImport = new PermissionWrapperComponent(this.appConfig, this.cd);
        permissionWrapperForImport.permissions = [this.appConfig.Operations.Add.toString()];
        permissionWrapperForImport.hasNotPermission.pipe(takeUntil(this._destroy$)).subscribe(res => {
            if (res)
                this.hasPermissionToImport = false;
        });

        const permissionWrapperForExport = new PermissionWrapperComponent(this.appConfig, this.cd);
        permissionWrapperForExport.permissions = [this.appConfig.Operations.Download.toString()];
        permissionWrapperForExport.hasNotPermission.pipe(takeUntil(this._destroy$)).subscribe(res => {
            if (res)
                this.hasPermissionToExport = false;
        });
    }

    ngAfterViewInit(): void { }

    protected export() {
        this.isExport.next(true);
    }

    protected columnSelector() {
        this.isColumnSelector.next(true);
    }

    protected import() {
        this.isImport.next(true);
    }

    protected sampleFileDownload() {
        this.isImportFileDownload.next(true);
    }

    protected bulkDelete() {
        this.bulkDeleteService.toggleCheckboxes(true);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}