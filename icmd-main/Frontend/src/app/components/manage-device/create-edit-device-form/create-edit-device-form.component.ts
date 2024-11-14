import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { FormArray, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { AttributeValueDtoModel, CreateOrEditDeviceDtoModel, DeviceDropdownInfoDtoModel } from "./create-edit-device-form.model";
import { DeviceModelService } from "src/app/service/device-model";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { DropdownInfoDtoModel } from "@m/common";
import { BoolType } from "@e/common";
import { ReferenceDocumentService } from "src/app/service/reference-document";
import { SystemService } from "src/app/service/system";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";
import { SubSystemInfoDtoModel } from "@c/masters/sub-system/list-sub-system-table";
import { SubSystemService } from "src/app/service/sub-system";
import { FormBaseComponent } from "@c/shared/forms";
import { FormGroupG, getGroup } from "@u/forms";
import { ReferenceDocumentInfoDtoModel } from "@c/masters/reference-document/list-reference-document-table";
import { IsInstrumentOption } from "@e/instrument";
import { ToastrService } from "ngx-toastr";
import { DeviceService } from "src/app/service/device";
import { CommonFunctions } from "@u/helper";
import { AppConfig } from "src/app/app.config";
import { AppRoute } from "@u/app.route";

@Component({
    standalone: true,
    selector: "app-create-edit-device-form",
    templateUrl: "./create-edit-device-form.component.html",
    imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        ReactiveFormsModule,
        CommonModule,
        MatIconModule, MatSelectModule, MatButtonModule],
    providers: [DeviceModelService, ReferenceDocumentService, SystemService, SubSystemService],
})
export class CreateOrEditDeviceFormComponent extends FormBaseComponent<CreateOrEditDeviceDtoModel> {
    deviceInfoData: DeviceDropdownInfoDtoModel;
    projectId: string = null;
    protected deviceModelInfo: DropdownInfoDtoModel[] = [];
    protected documentsInfo: DropdownInfoDtoModel[] = [];
    protected systemInfo: SystemInfoDtoModel[] = [];
    protected subSystemInfo: SubSystemInfoDtoModel[] = [];
    protected referenceDocuments: ReferenceDocumentInfoDtoModel[] = [];
    protected isAdd: boolean = false;
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    protected boolProperties: boolean | null = null;
    private _destroy$ = new Subject<void>();

    constructor(private _deviceModelService: DeviceModelService, private _referenceDocumentService: ReferenceDocumentService,
        private _systemService: SystemService, private _subSystemService: SubSystemService, private _cdr: ChangeDetectorRef,
        private _appConfig: AppConfig,
        private _toatsr: ToastrService, private _deviceService: DeviceService, protected _commonFunctions: CommonFunctions) {
        super(
            getGroup<CreateOrEditDeviceDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    selectReferenceDocTypeId: {},
                    selectedReferenceDocId: {},
                    referenceDocumentIds: { v: [] },
                    connectionParentTagId: {},
                    instrumentParentTagId: {},
                    deviceModelId: {},
                    projectId: {},
                    deviceTypeId: { vldtr: [Validators.required] },
                    failStateId: {},
                    historicalLogging: { v: null },
                    historicalLoggingFrequency: {},
                    historicalLoggingResolution: {},
                    isInstrument: { v: _appConfig.isPreviousURL$ == AppRoute.instrumentList ? IsInstrumentOption.Yes : IsInstrumentOption.No },
                    junctionBoxTagId: {},
                    lineVesselNumber: {},
                    manufacturerId: {},
                    natureOfSignalId: {},
                    panelTagId: {},
                    revisionChanges: {},
                    serialNumber: {},
                    service: {},
                    serviceBankId: {},
                    serviceDescription: {},
                    serviceTrainId: {},
                    serviceZoneId: {},
                    skidTagId: {},
                    standTagId: {},
                    subSystemId: {},
                    systemId: {},
                    tagId: { vldtr: [Validators.required] },
                    variable: {},
                    vendorSupply: { v: null },
                    workAreaPackId: {},
                    referenceDocumentInfo: { v: [] },
                    attributes: new FormArray<FormGroupG<AttributeValueDtoModel>>([]),
                    connectionCableTagId: {},
                    instrumentCableTagId: {},
                }
            )
        );
    }
    public get value(): CreateOrEditDeviceDtoModel | null {
        if (this.form.invalid) {
            this.form.controls['attributes'].markAllAsTouched();
            this.showErrors();
            return;
        }
        return super.value;
    }

    @Input() public set items(val: CreateOrEditDeviceDtoModel) {
        if (val) {
            const totalRefDocs = val?.referenceDocumentInfo.length;
            val.selectReferenceDocTypeId = totalRefDocs != 0 ? val?.referenceDocumentInfo[totalRefDocs - 1].referenceDocumentTypeId : null;
            val.selectedReferenceDocId = totalRefDocs != 0 ? val?.referenceDocumentInfo[totalRefDocs - 1].id : null;
            this.referenceDocuments = val?.referenceDocumentInfo;
            super.value = {
                ...val,
                attributes: []
            };
            if (val.manufacturerId) { this.getDeviceModelInfo(val.manufacturerId, val.deviceModelId); }

            if (val.selectReferenceDocTypeId)
                this.getDocumentInfo(val.selectReferenceDocTypeId, val.selectedReferenceDocId);

            if (val.workAreaPackId)
                this.getSystemInfo(val.workAreaPackId, val.systemId);

            if (val.systemId)
                this.getSubSystemInfo(val.systemId, val.subSystemId);

            this._cdr.detectChanges();
            this.setAttributes(val.attributes);
        }
    }

    protected getDeviceModelInfo(manufacturerId: string, modelId: string = null): void {
        this.field('deviceModelId').setValue(null);
        this.field('deviceModelId').updateValueAndValidity();
        if (manufacturerId) {
            this._deviceModelService.getDeviceInfoFromManufacturerId(manufacturerId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.deviceModelInfo = res;
                    if (modelId != null) {
                        this.field('deviceModelId').setValue(modelId);
                        this.field('deviceModelId').updateValueAndValidity();
                    }
                })
        }
        else {
            this.deviceModelInfo = [];
        }
    }

    protected getDocumentInfo(documentTypeId: string, docId: string = null): void {
        this.field('selectedReferenceDocId').setValue(null);
        this.field('selectedReferenceDocId').updateValueAndValidity();
        if (this.projectId && documentTypeId) {
            this._referenceDocumentService.getAllDocumentInfo(this.projectId, documentTypeId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.documentsInfo = res;
                    if (docId != null) {
                        this.field('selectedReferenceDocId').setValue(docId);
                        this.field('selectedReferenceDocId').updateValueAndValidity();
                    }
                })
        }
        else {
            this.documentsInfo = [];
        }
    }

    protected getSystemInfo(workAreaPackId: string, systemId: string = null): void {
        this.field('systemId').setValue(null);
        this.field('systemId').updateValueAndValidity();
        if (workAreaPackId) {
            this._systemService.getAllSystemInfo(this.projectId, workAreaPackId)
                .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                    this.systemInfo = res;
                    if (systemId != null) {
                        this.field('systemId').setValue(systemId);
                        this.field('systemId').updateValueAndValidity();
                    }
                })
        }
        else {
            this.systemInfo = [];
        }
    }

    protected getSubSystemInfo(systemId: string, subSystemId: string = null): void {
        this.field('subSystemId').setValue(null);
        this.field('subSystemId').updateValueAndValidity();
        if (systemId) {
            this._subSystemService.getAllSubSystemInfo(systemId)
                .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                    this.subSystemInfo = res;
                    if (subSystemId != null) {
                        this.field('subSystemId').setValue(subSystemId);
                        this.field('subSystemId').updateValueAndValidity();
                    }
                })
        }
        else {
            this.subSystemInfo = [];
        }
    }

    protected addReferenceDocument(): void {
        const refDocumentId = this.field('selectedReferenceDocId').value;
        if (refDocumentId) {
            if (this.referenceDocuments.length > 0 && this.referenceDocuments.some(x => x.id == refDocumentId)) {
                this._toatsr.error("This reference document already exists.")
                return;
            }

            this._referenceDocumentService.getReferenceDocumentInfo(refDocumentId)
                .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                    this.referenceDocuments.push(res);
                    this.field('referenceDocumentIds').setValue(this.referenceDocuments.map(x => x.id));
                    this._cdr.detectChanges();
                })
        }
    }

    protected deleteReferenceDocument(index: number): void {
        this.referenceDocuments.splice(index, 1);
        this.field('referenceDocumentIds').setValue(this.referenceDocuments.map(x => x.id));
        this._cdr.detectChanges();
    }

    protected getAttributeDefinitionIdsFromTypes(): void {
        const deviceTypeId = this.field('deviceTypeId').value ?? null;
        const deviceModelId = this.field('deviceModelId').value ?? null;
        const natureOfSignalId = this.field('natureOfSignalId').value ?? null;
        const connectionParentTagId = this.field('connectionParentTagId').value;
        const info = {
            deviceTypeId: deviceTypeId == "" ? null : deviceTypeId,
            deviceModelId: deviceModelId == "" ? null : deviceModelId,
            natureOfSignalId: natureOfSignalId == "" ? null : natureOfSignalId,
            projectId: this.projectId,
            deviceId: this.field('id').value,
            connectionParentTagId: connectionParentTagId == "" ? null : connectionParentTagId,
        }
        this._deviceService.getAttributes(info)
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                this.setAttributes(res);
                this._cdr.detectChanges();
            })
    }

    private setAttributes(attributes: AttributeValueDtoModel[]): void {
        const attributeslist = this.array("attributes");
        attributeslist.clear();
        attributes.map((res) => {
            attributeslist.push(
                getGroup<AttributeValueDtoModel>({
                    id: { v: res?.id ?? this.emptyGuid },
                    name: { v: res?.name ?? "" },
                    valueType: { v: res?.valueType ?? "" },
                    value: { v: res?.value ?? "", vldtr: res?.required ? [Validators.required] : [] },
                    required: { v: res?.required ?? false }
                })
            );
        })
        this._cdr.detectChanges();
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}