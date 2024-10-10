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
import { EquipmentCodeService } from "src/app/service/equipmentCode";
import { CreateOrEditEquipmentCodeFormComponent } from "@c/masters/equipmentCode/create-edit-equipmentCode-form";

@Component({
    standalone: true,
    selector: "app-equipmentCode-dialog",
    templateUrl: "./equipmentCode-add-edit-dialog.component.html",
    providers: [
        EquipmentCodeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditEquipmentCodeFormComponent,
        MatDialogModule
    ],
})
export class EquipmentCodeAddEditDialogComponent {
    @ViewChild(CreateOrEditEquipmentCodeFormComponent) equipmentCodeForm: CreateOrEditEquipmentCodeFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            EquipmentCodeAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _equipmentCodeservice: EquipmentCodeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._equipmentCodeservice.getEquipmentCodeInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.equipmentCodeForm.value =
                    {
                        id: res?.id,
                        code: res?.code,
                        descriptor: res?.descriptor
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveCodeInfo(): void {
        const codeInfo = this.equipmentCodeForm.value;
        if (codeInfo === null || codeInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._equipmentCodeservice.createEditEquipmentCode(codeInfo).subscribe(
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