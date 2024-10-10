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
import { ManufacturerService } from "src/app/service/manufacturer";
import { CreateOrEditManufacturerFormComponent } from "@c/masters/manufacturer/create-edit-manufacturer-form";

@Component({
    standalone: true,
    selector: "app-manufacturer-dialog",
    templateUrl: "./manufacturer-add-edit-dialog.component.html",
    providers: [
        ManufacturerService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditManufacturerFormComponent,
        MatDialogModule
    ],
})
export class ManufacturerAddEditDialogComponent {
    @ViewChild(CreateOrEditManufacturerFormComponent) manufacturerForm: CreateOrEditManufacturerFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
        ManufacturerAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _manufacturerService: ManufacturerService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._manufacturerService.getManufacturerInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.manufacturerForm.value =
                    {
                        id: res?.id,
                        name: res?.name,
                        description: res?.description,
                        comment: res?.comment
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveManufacturerInfo(): void {
        const manufacturerInfo = this.manufacturerForm.value;
        if (manufacturerInfo === null || manufacturerInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._manufacturerService.createEditManufacturer(manufacturerInfo).subscribe(
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