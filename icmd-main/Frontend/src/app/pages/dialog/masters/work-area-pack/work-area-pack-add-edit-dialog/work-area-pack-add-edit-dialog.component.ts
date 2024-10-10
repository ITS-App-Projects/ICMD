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
import { CreateOrEditWorkAreaPackFormComponent } from "@c/masters/workAreaPack/create-edit-work-area-form";
import { WorkAreaPackService } from "src/app/service/workAreaPack";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

@Component({
    standalone: true,
    selector: "app-work-area-pack-dialog",
    templateUrl: "./work-area-pack-add-edit-dialog.component.html",
    providers: [
        WorkAreaPackService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditWorkAreaPackFormComponent,
        MatDialogModule
    ],
})
export class WorkAreaPackAddEditDialogComponent {
    @ViewChild(CreateOrEditWorkAreaPackFormComponent) workAreaPackForm: CreateOrEditWorkAreaPackFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            WorkAreaPackAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _workAreaPackService: WorkAreaPackService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.workAreaPackForm.field('projectId').setValue(this._inputData.projectId);
        }
        if (this._inputData.id != null) {
            this._workAreaPackService.getWorkAreaPackInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.workAreaPackForm.value =
                    {
                        id: res?.id,
                        number: res?.number,
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

    protected saveWorkAreaPackInfo(): void {
        const workAreaInfo = this.workAreaPackForm.value;
        if (workAreaInfo === null || workAreaInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._workAreaPackService.createEditWorkAreaPack(workAreaInfo).subscribe(
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