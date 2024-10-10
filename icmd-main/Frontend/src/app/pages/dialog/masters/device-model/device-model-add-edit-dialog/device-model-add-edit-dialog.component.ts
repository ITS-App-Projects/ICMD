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
import { CommonDialogOutputDataModel } from "@m/common";
import { DeviceModelService } from "src/app/service/device-model";
import { CreateOrEditDeviceModelFormComponent } from "@c/masters/device-model/create-edit-device-model-form";
import { DeviceModelDialogInputDataModel } from "./device-model-add-edit-dialog.model";

@Component({
    standalone: true,
    selector: "app-device-model-dialog",
    templateUrl: "./device-model-add-edit-dialog.component.html",
    providers: [
        DeviceModelService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditDeviceModelFormComponent,
        MatDialogModule
    ],
})
export class DeviceModelAddEditDialogComponent {
    @ViewChild(CreateOrEditDeviceModelFormComponent) deviceModelForm: CreateOrEditDeviceModelFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            DeviceModelAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: DeviceModelDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _deviceModelService: DeviceModelService
    ) { }

    ngAfterViewInit(): void {
        this.deviceModelForm.manufacturersData = this._inputData.manufacturers;
        if (this._inputData.id != null) {
            this._deviceModelService.getDeviceModelInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.deviceModelForm.items = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveModelInfo(): void {
        const modelForm = this.deviceModelForm.value;
        const attributeForm = this.deviceModelForm.attributeForm.value;
        if (modelForm === null || modelForm === undefined || attributeForm === null || attributeForm === undefined) {
            return;
        }

        modelForm.attributes = attributeForm.attributes;
        this.isLoading = !this.isLoading;
        this._deviceModelService.createEditDeviceModel(modelForm).subscribe(
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