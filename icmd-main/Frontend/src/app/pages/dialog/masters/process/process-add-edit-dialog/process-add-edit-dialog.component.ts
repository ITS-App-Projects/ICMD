import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { HttpErrorResponse } from "@angular/common/http";
import { takeUntil } from "rxjs/operators";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { ProcessService } from "src/app/service/process";
import { CreateOrEditProcessFormComponent } from "@c/masters/process/create-edit-process-form";

@Component({
    standalone: true,
    selector: "app-process-dialog",
    templateUrl: "./process-add-edit-dialog.component.html",
    providers: [
        ProcessService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditProcessFormComponent,
        MatDialogModule
    ],
})
export class ProcessAddEditDialogComponent {
    @ViewChild(CreateOrEditProcessFormComponent) processForm: CreateOrEditProcessFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            ProcessAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _processService: ProcessService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.processForm.field('projectId').setValue(this._inputData.projectId);
        }
        if (this._inputData.id != null) {
            this._processService.getProcessInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.processForm.value =
                    {
                        id: res?.id,
                        processName: res?.processName,
                        description: res?.description,
                        projectId: res?.projectId
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveProcessInfo(): void {
        const processInfo = this.processForm.value;
        if (processInfo === null || processInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._processService.createEditProcess(processInfo).subscribe(
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