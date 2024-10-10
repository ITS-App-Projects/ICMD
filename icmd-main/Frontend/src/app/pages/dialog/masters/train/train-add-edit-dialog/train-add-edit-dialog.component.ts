import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { TrainService } from "src/app/service/train";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { CreateOrEditTrainFormComponent } from "@c/masters/train/create-edit-train-form";
import { takeUntil } from "rxjs/operators";
import { HttpErrorResponse } from "@angular/common/http";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

@Component({
    standalone: true,
    selector: "app-train-dialog",
    templateUrl: "./train-add-edit-dialog.component.html",
    providers: [
        TrainService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditTrainFormComponent,
        MatDialogModule
    ],
})
export class TrainAddEditDialogComponent {
    @ViewChild(CreateOrEditTrainFormComponent) trainForm: CreateOrEditTrainFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            TrainAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _trainService: TrainService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.trainForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._trainService.getTrainInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.trainForm.value =
                    {
                        id: res?.id,
                        train: res?.train,
                        projectId: res?.projectId
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveTrainInfo(): void {
        const trainInfo = this.trainForm.value;
        if (trainInfo === null || trainInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._trainService.createEditTrain(trainInfo).subscribe(
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