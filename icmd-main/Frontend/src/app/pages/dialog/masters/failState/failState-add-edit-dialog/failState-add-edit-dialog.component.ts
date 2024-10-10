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
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { FailStateService } from "src/app/service/failState";
import { CreateOrEditFailStateFormComponent } from "@c/masters/failState/create-edit-failState-form";

@Component({
    standalone: true,
    selector: "app-failState-dialog",
    templateUrl: "./failState-add-edit-dialog.component.html",
    providers: [
        FailStateService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditFailStateFormComponent,
        MatDialogModule
    ],
})
export class FailStateAddEditDialogComponent {
    @ViewChild(CreateOrEditFailStateFormComponent) failStateForm: CreateOrEditFailStateFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            FailStateAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _failStateService: FailStateService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._failStateService.getFailStateInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.failStateForm.value =
                    {
                        id: res?.id,
                        failStateName: res?.failStateName
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveFailStateInfo(): void {
        const failStateInfo = this.failStateForm.value;
        if (failStateInfo === null || failStateInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._failStateService.createEditFailState(failStateInfo).subscribe(
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