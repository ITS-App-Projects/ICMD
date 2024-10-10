import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { CreateOrEditSystemFormComponent } from "@c/masters/system/create-edit-system-form";
import { CommonDialogOutputDataModel } from "@m/common";
import { Subject } from "rxjs";
import { SystemService } from "src/app/service/system";
import { SystemDialogInputDataModel } from "./system-add-edit-dialog.model";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { takeUntil } from "rxjs/operators";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
    standalone: true,
    selector: "app-system-dialog",
    templateUrl: "./system-add-edit-dialog.component.html",
    providers: [
        SystemService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditSystemFormComponent,
        MatDialogModule
    ],
})
export class SystemAddEditDialogComponent {
    @ViewChild(CreateOrEditSystemFormComponent) systemForm: CreateOrEditSystemFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            SystemAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: SystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _systemService: SystemService
    ) { }

    ngAfterViewInit(): void {
        this.systemForm.workAreaPacksData = this._inputData.workAreaPacks;
        if (this._inputData.id != null) {
            this._systemService.getSystemInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.systemForm.value =
                    {
                        id: res?.id,
                        number: res?.number,
                        description: res?.description,
                        workAreaPackId: res?.workAreaPackId,
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveSystemInfo(): void {
        const systemInfo = this.systemForm.value;
        if (systemInfo === null || systemInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._systemService.createEditSystem(systemInfo).subscribe(
            (res) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._dialogRef.close({ success: true });
                } else {
                    this.isLoading = !this.isLoading;
                    this._toastr.error(res.message);
                }
            },
            (errorRes: HttpErrorResponse) => {
                this.isLoading = !this.isLoading;
                if (errorRes?.error?.message) {
                    this._toastr.error(errorRes?.error?.message);
                }
            }
        );
    }


    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}