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
import { StreamService } from "src/app/service/stream";
import { CreateOrEditStreamFormComponent } from "@c/masters/stream/create-edit-stream-form";

@Component({
    standalone: true,
    selector: "app-stream-dialog",
    templateUrl: "./stream-add-edit-dialog.component.html",
    providers: [
        StreamService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditStreamFormComponent,
        MatDialogModule
    ],
})
export class StreamAddEditDialogComponent {
    @ViewChild(CreateOrEditStreamFormComponent) streamForm: CreateOrEditStreamFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            StreamAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _streamService: StreamService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.streamForm.field('projectId').setValue(this._inputData.projectId);
        }
        if (this._inputData.id != null) {
            this._streamService.getStreamInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.streamForm.value =
                    {
                        id: res?.id,
                        streamName: res?.streamName,
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

    protected saveStreamInfo(): void {
        const streamInfo = this.streamForm.value;
        if (streamInfo === null || streamInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._streamService.createEditStream(streamInfo).subscribe(
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