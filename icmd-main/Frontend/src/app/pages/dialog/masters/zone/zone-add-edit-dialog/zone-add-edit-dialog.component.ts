import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { CreateOrEditZoneFormComponent } from "@c/masters/zone/create-edit-zone-form";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { ProgressBarService } from "src/app/service/common";
import { ZoneService } from "src/app/service/zone";

@Component({
    standalone: true,
    selector: "app-zone-dialog",
    templateUrl: "./zone-add-edit-dialog.component.html",
    providers: [
        ZoneService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditZoneFormComponent,
        MatDialogModule
    ],
})
export class ZoneAddEditDialogComponent {
    @ViewChild(CreateOrEditZoneFormComponent) zoneForm: CreateOrEditZoneFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            ZoneAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _zoneService: ZoneService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.zoneForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._zoneService.getZoneInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.zoneForm.value =
                    {
                        id: res?.id,
                        zone: res?.zone,
                        description: res?.description,
                        area: res?.area ? parseInt(res?.area) : null,
                        projectId: res?.projectId
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveZoneInfo(): void {
        const zoneInfo = this.zoneForm.value;
        if (zoneInfo === null || zoneInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._zoneService.createEditZone(zoneInfo).subscribe(
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