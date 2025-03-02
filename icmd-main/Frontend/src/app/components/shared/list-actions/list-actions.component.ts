import {
  takeUntil,
  Subject
} from 'rxjs';
import { AppConfig } from 'src/app/app.config';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';

import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RoleEnum } from '@e/common/role.enum';

import { PermissionWrapperComponent } from '../permission-wrapper';

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
    @Input() context!: string;
    @Output() isExport = new EventEmitter<boolean>(false);
    @Output() isColumnSelector = new EventEmitter<boolean>(false);
    @Output() isImport = new EventEmitter<boolean>(false);
    @Output() isImportFileDownload = new EventEmitter<boolean>(false);
    @Output() isBulkEdit = new EventEmitter<boolean>(false);

    protected hasPermissionToImport: boolean = true;
    protected hasPermissionToExport: boolean = true;
    protected hasPermissionToBulkDelete: boolean = true;
    protected hasPersmissionToBulkEdit: boolean = true;
    private _destroy$: Subject<void> = new Subject<void>();

    constructor(public appConfig: AppConfig, private cd: ChangeDetectorRef, private bulkDeleteService: BulkDeleteService) {
     }

    ngOnInit() {
        // Wait for the page project id to load
        this.appConfig.projectIdFilter$.pipe(takeUntil(this._destroy$)).subscribe((res) => {
            const permissionWrapperForImport = new PermissionWrapperComponent(this.appConfig, this.cd);
            permissionWrapperForImport.permissions = [this.appConfig.Operations.Add.toString()];
            this.hasPermissionToImport = permissionWrapperForImport.checkPermission();
            permissionWrapperForImport.hasNotPermission.pipe(takeUntil(this._destroy$)).subscribe(res => {
                if (res)
                    this.hasPermissionToImport = false;
            });
            const permissionWrapperForExport = new PermissionWrapperComponent(this.appConfig, this.cd);
            permissionWrapperForExport.permissions = [this.appConfig.Operations.Download.toString()];
            this.hasPermissionToExport = permissionWrapperForExport.checkPermission();
            permissionWrapperForExport.hasNotPermission.pipe(takeUntil(this._destroy$)).subscribe(res => {
                if (res)
                    this.hasPermissionToExport = false;
            });

            const permissionWrapperForBulkDelete = new PermissionWrapperComponent(this.appConfig, this.cd);
            permissionWrapperForBulkDelete.permissionByRole = [RoleEnum.Administrator, RoleEnum.SuperUser];
            this.hasPermissionToBulkDelete = permissionWrapperForBulkDelete.checkPermission();
            permissionWrapperForBulkDelete.hasNotPermission.pipe(takeUntil(this._destroy$)).subscribe(res => {
                if (res)
                    this.hasPermissionToBulkDelete = false;
            });

            const permissionWrapperForBulkEdit = new PermissionWrapperComponent(this.appConfig, this.cd);
            permissionWrapperForBulkEdit.permissions = [this.appConfig.Operations.Edit.toString()];
            this.hasPersmissionToBulkEdit = permissionWrapperForBulkEdit.checkPermission();
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

    protected bulkEdit() {
        this.isBulkEdit.next(true);
    }

    protected bulkDelete() {
        this.bulkDeleteService.toggleBulkDelete(this.context, true);
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}