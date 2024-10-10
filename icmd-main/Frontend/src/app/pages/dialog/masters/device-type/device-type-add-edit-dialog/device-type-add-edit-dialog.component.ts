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
import { DeviceTypeService } from "src/app/service/device-type";
import { CreateOrEditDeviceTypeFormComponent } from "@c/masters/device-type/create-edit-device-type-form";

@Component({
    standalone: true,
    selector: "app-device-type-dialog",
    templateUrl: "./device-type-add-edit-dialog.component.html",
    providers: [
        DeviceTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditDeviceTypeFormComponent,
        MatDialogModule
    ],
})
export class DeviceTypeAddEditDialogComponent {
    @ViewChild(CreateOrEditDeviceTypeFormComponent) deviceTypeForm: CreateOrEditDeviceTypeFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            DeviceTypeAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _deviceTypeService: DeviceTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._deviceTypeService.getDeviceTypeInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.deviceTypeForm.items = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveTypeInfo(): void {
        const typeForm = this.deviceTypeForm.value;
        const attributeForm = this.deviceTypeForm.attributeForm.value;
        if (typeForm === null || typeForm === undefined || attributeForm === null || attributeForm === undefined) {
            return;
        }

        typeForm.attributes = attributeForm.attributes;
        this.isLoading = !this.isLoading;
        this._deviceTypeService.createEditDeviceType(typeForm).subscribe(
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