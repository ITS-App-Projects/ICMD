import { CommonModule } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { ViewDeviceDetailComponent } from "@c/manage-device/view-device-detail";
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { DeviceService } from "src/app/service/device";

@Component({
    standalone: true,
    selector: "app-device-dialog",
    templateUrl: "./device-view-dialog.component.html",
    providers: [
        DeviceService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        ViewDeviceDetailComponent,
        MatDialogModule
    ],
})
export class DeviceViewDialogComponent implements AfterViewInit {
    @ViewChild(ViewDeviceDetailComponent) viewDevice: ViewDeviceDetailComponent;
    private _destroy$ = new Subject<void>();

    constructor(private _dialogRef: MatDialogRef<
        DeviceViewDialogComponent,
        CommonDialogOutputDataModel
    >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _deviceService: DeviceService,
        private _cdr: ChangeDetectorRef) {

    }
    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._deviceService.viewDeviceInfoById(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.viewDevice.deviceData = res;
                    this._cdr.detectChanges();
                })

        }
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}