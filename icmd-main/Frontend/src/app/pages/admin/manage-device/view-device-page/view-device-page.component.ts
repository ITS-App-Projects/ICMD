import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input, ViewChild } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { ViewDeviceDetailComponent } from "@c/manage-device/view-device-detail";
import { FormDefaultsModule } from "@c/shared/forms";
import { AppRoute } from "@u/app.route";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { DeviceService } from "src/app/service/device";

@Component({
    standalone: true,
    selector: "app-view-device-page",
    templateUrl: "./view-device-page.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        ViewDeviceDetailComponent
    ],
    providers: [DeviceService]
})
export class ViewDevicePageComponent {
    @ViewChild(ViewDeviceDetailComponent) viewDevice: ViewDeviceDetailComponent;
    @Input() deviceId!: string;
    private _destroy$ = new Subject<void>();

    constructor(private _router: Router,
        private _deviceService: DeviceService, private _appConfig: AppConfig, private _cdr: ChangeDetectorRef) {

    }

    ngAfterViewInit(): void {
        this.viewDeviceInfo();
    }

    protected updateDevice(): void {
        if (this.deviceId) {
            this._appConfig.isPreviousURL$ = AppRoute.viewDevice + "/" + this.deviceId;
            this._router.navigate([AppRoute.manageDevice, this.deviceId ?? ""]);
        }
    }

    protected manageInstruments(): void {
        this._router.navigate([AppRoute.manageHierarchy]);
    }

    private viewDeviceInfo(): void {
        if (this.deviceId != null) {
            this._deviceService.viewDeviceInfoById(this.deviceId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.viewDevice.deviceData = res;
                    this._cdr.detectChanges();
                })

        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }

}