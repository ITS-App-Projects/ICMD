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
import { SubProcessService } from "src/app/service/sub-process";
import { CreateOrEditSubProcessFormComponent } from "@c/masters/sub-process/create-edit-sub-process-form";

@Component({
    standalone: true,
    selector: "app-sub-process-dialog",
    templateUrl: "./sub-process-add-edit-dialog.component.html",
    providers: [
        SubProcessService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditSubProcessFormComponent,
        MatDialogModule
    ],
})
export class SubProcessAddEditDialogComponent {
    @ViewChild(CreateOrEditSubProcessFormComponent) subProcessForm: CreateOrEditSubProcessFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            SubProcessAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _subPprocessService: SubProcessService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.subProcessForm.field('projectId').setValue(this._inputData.projectId);
        }
        if (this._inputData.id != null) {
            this._subPprocessService.getSubProcessInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.subProcessForm.value =
                    {
                        id: res?.id,
                        subProcessName: res?.subProcessName,
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

    protected saveSubProcessInfo(): void {
        const processInfo = this.subProcessForm.value;
        if (processInfo === null || processInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._subPprocessService.createEditSubProcess(processInfo).subscribe(
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