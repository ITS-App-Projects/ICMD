import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { DeviceService } from 'src/app/service/device';
import { TagService } from 'src/app/service/tag';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  ViewChild
} from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import {
  CreateOrEditDeviceDtoModel,
  CreateOrEditDeviceFormComponent
} from '@c/manage-device/create-edit-device-form';
import { FormDefaultsModule } from '@c/shared/forms';
import { AppRoute } from '@u/app.route';

@Component({
    standalone: true,
    selector: "app-list-device-page",
    templateUrl: "./list-device-page.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule,
        CreateOrEditDeviceFormComponent
    ],
    providers: [DeviceService, TagService]
})
export class ListDevicePageComponent {
    @ViewChild(CreateOrEditDeviceFormComponent) deviceForm: CreateOrEditDeviceFormComponent;
    private projectId: string | null = null;
    @Input() deviceId!: string;
    private _destroy$ = new Subject<void>();

    constructor(private _deviceService: DeviceService,
        private _appConfig: AppConfig,
        private _tagService: TagService,
        private _toastr: ToastrService,
        private _router: Router,
        private _cdr: ChangeDetectorRef) {
        this.projectId = this._appConfig.currentProjectId;
    }

    ngAfterViewInit(): void {
        this.deviceForm.projectId = this.projectId;
        this.getDeviceDropdownInfo()


        if (this.projectId) {
            this.deviceForm.field("projectId").setValue(this.projectId);
        }


        this._appConfig.projectIdFilter$.subscribe((res) => {
            if (res && this.projectId != res?.id) {
                this.projectId = res?.id;
                this.deviceForm.projectId = this.projectId;
                this.getDeviceDropdownInfo();
            }
        });
    }

    protected SaveDevice() {
        this.deviceForm.field("projectId").setValue(this.projectId);
        const formValue = this.deviceForm.value;
        if (formValue == null)
            return;

        const guidProperties: string[] = ["manufacturerId", "deviceModelId", "failStateId", "serviceZoneId", "serviceBankId", "serviceTrainId", "natureOfSignalId", "skidTagId", "panelTagId", "junctionBoxTagId", "standTagId", "workAreaPackId", "systemId", "subSystemId", "historicalLoggingFrequency", "historicalLoggingResolution", "connectionParentTagId", "instrumentParentTagId", "connectionCableTagId", "instrumentCableTagId"];

        Object.keys(formValue).forEach(element => {
            if (guidProperties.some(x => x == element)) {
                if (formValue[element] == "")
                    //this.deviceForm.field(element).setValue(null);
                    formValue[element] = null;
            }
        });

        if (formValue.connectionParentTagId != null && formValue.instrumentParentTagId != null && formValue.connectionParentTagId === formValue.instrumentParentTagId) {
            this._toastr.error("Connection and Instrument parent device can't be the same.");
            return false;
        }

        if (formValue.connectionCableTagId != null && formValue.instrumentCableTagId != null && formValue.connectionCableTagId === formValue.instrumentCableTagId) {
            this._toastr.error("Origin and Destination cable device can't be the same.");
            return false;
        }

        if (formValue.historicalLogging?.toString() === "") {
            formValue.historicalLogging = null;
        }

        if (formValue.vendorSupply?.toString() === "") {
            formValue.vendorSupply = null;
        }

        this._deviceService.createOrEditDevice(formValue)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._router.navigate([this._appConfig.isPreviousURL$ != null ? this._appConfig.isPreviousURL$ : AppRoute.instrumentList]);
                } else {
                    this._toastr.error(res.message);
                }
            });
    }

    protected manageInstruments(): void {
        this._router.navigate([this._appConfig.isPreviousURL$]);
    }

    private getDeviceDropdownInfo(): void {
        if (this.projectId) {
            this._deviceService.getDeviceDropdownInfo(this.projectId, this.deviceId)
                .pipe(takeUntil(this._destroy$)).subscribe((deviceInfo) => {
                    this.deviceForm.deviceInfoData = deviceInfo;
                    const formProjectId = this.deviceForm.field('projectId').value;
                    if (formProjectId != this.projectId) {
                        this.deviceForm.resetForm();
                        this.deviceForm.field('projectId').setValue(this.projectId);
                    }
                    if (this.deviceId) {
                        this.getDeviceById(this.deviceId);
                    }
                    this._cdr.detectChanges();
                });
        }
    }

    private getDeviceById(id: string) {
        this._deviceService.getDeviceInfoById(id)
            .pipe(takeUntil(this._destroy$)).subscribe((res: CreateOrEditDeviceDtoModel) => {
                this.deviceForm.items = res;
                this._cdr.detectChanges();
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}