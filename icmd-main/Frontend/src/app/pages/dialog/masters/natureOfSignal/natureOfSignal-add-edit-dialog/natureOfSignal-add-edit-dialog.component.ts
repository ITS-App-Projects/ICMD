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
import { NatureOfSignalService } from "src/app/service/natureOfSignal";
import { CreateOrEditNatureOfSignalFormComponent } from "@c/masters/natureOfSignal/create-edit-natureOfSignal-form";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";

@Component({
    standalone: true,
    selector: "app-natureOfSignal-dialog",
    templateUrl: "./natureOfSignal-add-edit-dialog.component.html",
    providers: [
        NatureOfSignalService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditNatureOfSignalFormComponent,
        MatDialogModule
    ],
})
export class NatureOfSignalAddEditDialogComponent {
    @ViewChild(CreateOrEditNatureOfSignalFormComponent) natureOfSignalForm: CreateOrEditNatureOfSignalFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
        NatureOfSignalAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _natureOfSignalService: NatureOfSignalService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._natureOfSignalService.getNatureOfSignalInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.natureOfSignalForm.items = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveSignalInfo(): void {
        const typeForm = this.natureOfSignalForm.value;
        const attributeForm = this.natureOfSignalForm.attributeForm.value;
        if (typeForm === null || typeForm === undefined || attributeForm === null || attributeForm === undefined) {
            return;
        }

        typeForm.attributes = attributeForm.attributes;
        this.isLoading = !this.isLoading;
        this._natureOfSignalService.createEditNatureOfSignal(typeForm).subscribe(
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