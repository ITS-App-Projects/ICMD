import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input, ViewChild } from "@angular/core";
import { ChangeLogInfoDtoModel, ChangeLogListDtoModel, ViewAuditLogReportComponent } from "@c/manage-reports/view-auditLog-report";
import { DPPADevicesDtoModel, ViewDPPADevicesReportComponent } from "@c/manage-reports/view-dppa-devices-report";
import { DuplicateDPNodeAddressDtoModel, ViewDuplicateDPNodeReportComponent, ViewInstrumentListLiveDtoModel } from "@c/manage-reports/view-duplicate-dpnode-report";
import { DuplicatePANodeAddressDtoModel, ViewDuplicatePANodeReportComponent } from "@c/manage-reports/view-duplicate-panode-report";
import { DuplicateRackSlotChannelDtoModel, ViewDuplicateRackSlotReportComponent } from "@c/manage-reports/view-duplicate-rack-slot-report";
import { PnIdDeviceDocumentReferenceRequestDtoModel, ViewPnIDDeviceDocumentReferenceCompareDtoModel, ViewPnIdDeviceDocumentReferenceReportComponent } from "@c/manage-reports/view-pnId-device-documentReference-report";
import { PnIDTagExceptionInfoDtoModel, ViewPnIDTagExceptionDtoModel, ViewPnIdTagExceptionReportComponent } from "@c/manage-reports/view-pnId-tag-exception-report";
import { PnIdDeviceMisMatchDocumentReference, ReportTypes } from "@e/common";
import { ExcelHelper } from "@u/helper";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ReportService } from "src/app/service/report";
import { ReportInfoDtoModel, ReportListDtoModel } from "../list-reports-page";
import { ViewInstrumentListReportComponent } from "@c/manage-reports/view-instrument-list-report";
import { ViewNonInstrumentListReportComponent } from "@c/manage-reports/view-non-instrument-list-report";
import { ViewNonInstrumentListDtoModel } from "@c/nonInstrument-list/list-nonInstrument-table";
import { ViewOMItemInstrumentListDtoModel, ViewOMItemInstrumentReportComponent } from "@c/manage-reports/view-om-item-instrument-report";
import { SparesReportResponceDtoModel, ViewSparesReportComponent } from "@c/manage-reports/view-spares-report";
import { SparesReportDetailsResponceDtoModel, ViewSparesDetailReportComponent } from "@c/manage-reports/view-spares-detail-report";
import { SparesReportPLCResponceDtoModel, ViewSparesPLCReportComponent } from "@c/manage-reports/view-spares-plc-report";
import { UnassociatedTagsDtoModel, ViewUnassociatedTagsReportComponent } from "@c/manage-reports/view-unassociated-tags-report";
import { ViewUnassociatedSkidsDto, ViewUnassociatedSkidsReportComponent } from "@c/manage-reports/view-unassociated-skids-report";
import { ViewNatureOfSignalValidationFailuresDtoModel, ViewNatureOfSignalValidationsReportComponent } from "@c/manage-reports/view-nature-of-signal-validations-report";
import { ViewPSSTagsDtoModel, ViewPSSTagsReportComponent } from "@c/manage-reports/view-pss-tags-report";

@Component({
    standalone: true,
    selector: "app-view-report-page",
    templateUrl: "./view-report-page.component.html",
    imports: [
        CommonModule,
        ViewAuditLogReportComponent,
        ViewDPPADevicesReportComponent,
        ViewDuplicateDPNodeReportComponent,
        ViewDuplicatePANodeReportComponent,
        ViewDuplicateRackSlotReportComponent,
        ViewPnIdTagExceptionReportComponent,
        ViewPnIdDeviceDocumentReferenceReportComponent,
        ViewInstrumentListReportComponent,
        ViewNonInstrumentListReportComponent,
        ViewOMItemInstrumentReportComponent,
        ViewSparesReportComponent,
        ViewSparesDetailReportComponent,
        ViewSparesPLCReportComponent,
        ViewUnassociatedTagsReportComponent,
        ViewUnassociatedSkidsReportComponent,
        ViewNatureOfSignalValidationsReportComponent,
        ViewPSSTagsReportComponent
    ],
    providers: [ReportService, ExcelHelper]
})
export class ViewReportPageComponent {
    @ViewChild(ViewAuditLogReportComponent) auditLogReport: ViewAuditLogReportComponent;
    @ViewChild(ViewDPPADevicesReportComponent) dppaDevicesReport: ViewDPPADevicesReportComponent;
    @ViewChild(ViewDuplicateDPNodeReportComponent) dpNodeReport: ViewDuplicateDPNodeReportComponent;
    @ViewChild(ViewDuplicatePANodeReportComponent) paNodeReport: ViewDuplicatePANodeReportComponent;
    @ViewChild(ViewDuplicateRackSlotReportComponent) rackSlotReport: ViewDuplicateRackSlotReportComponent;
    @ViewChild(ViewPnIdTagExceptionReportComponent) pnIdTagReport: ViewPnIdTagExceptionReportComponent;
    @ViewChild(ViewPnIdDeviceDocumentReferenceReportComponent) pnIdDocumentReport: ViewPnIdDeviceDocumentReferenceReportComponent;
    @ViewChild(ViewInstrumentListReportComponent) instrumentReport: ViewInstrumentListReportComponent;
    @ViewChild(ViewNonInstrumentListReportComponent) nonInstrumentReport: ViewNonInstrumentListReportComponent;
    @ViewChild(ViewOMItemInstrumentReportComponent) omInstrumentReport: ViewOMItemInstrumentReportComponent;
    @ViewChild(ViewSparesReportComponent) sparesReport: ViewSparesReportComponent;
    @ViewChild(ViewSparesDetailReportComponent) sparesDetailReport: ViewSparesDetailReportComponent;
    @ViewChild(ViewSparesPLCReportComponent) sparesPLCReport: ViewSparesPLCReportComponent;
    @ViewChild(ViewUnassociatedTagsReportComponent) tagsReport: ViewUnassociatedTagsReportComponent;
    @ViewChild(ViewUnassociatedSkidsReportComponent) skidsReport: ViewUnassociatedSkidsReportComponent;
    @ViewChild(ViewNatureOfSignalValidationsReportComponent) natureOfSignalReport: ViewNatureOfSignalValidationsReportComponent;
    @ViewChild(ViewPSSTagsReportComponent) pssTagsReport: ViewPSSTagsReportComponent;
    @Input() name!: string;
    protected title: string | null = null;
    protected subTitle: string | null = null;
    protected reportType = ReportTypes;
    protected isExistRecords: boolean = false;
    private projectId: string | null = null;
    private auditLogData: ChangeLogListDtoModel[] = [];
    private dppaDeviceData: DPPADevicesDtoModel[] = [];
    private dpNodeData: DuplicateDPNodeAddressDtoModel[] = [];
    private paNodeData: DuplicatePANodeAddressDtoModel[] = [];
    private rackSlotChannelData: DuplicateRackSlotChannelDtoModel[] = [];
    private pnIdTagExceptionData: PnIDTagExceptionInfoDtoModel[] = [];
    private pnIdDeviceDocumentData: ViewPnIDDeviceDocumentReferenceCompareDtoModel[] = [];
    private instrumentListData: ViewInstrumentListLiveDtoModel[] = [];
    private nonInstrumentListData: ViewNonInstrumentListDtoModel[] = [];
    private omIteminstrumentListData: ViewOMItemInstrumentListDtoModel[] = [];
    private sparesListData: SparesReportResponceDtoModel[] = [];
    private sparesDetailsListData: SparesReportDetailsResponceDtoModel[] = [];
    private sparesPLCListData: SparesReportPLCResponceDtoModel[] = [];
    private tagListData: UnassociatedTagsDtoModel[] = [];
    private skidListData: ViewUnassociatedSkidsDto[] = [];
    private natureListData: ViewNatureOfSignalValidationFailuresDtoModel[] = [];
    private pssTagListData: ViewPSSTagsDtoModel[] = [];
    private reportList: ReportListDtoModel[] = [];
    private _destroy$ = new Subject<void>();

    constructor(private _cdr: ChangeDetectorRef,
        private _reportService: ReportService,
        private _excelHelper: ExcelHelper,
        private _appConfig: AppConfig) {
        this.getAllReports();
    }

    ngAfterViewInit(): void {
        this._appConfig.projectIdFilter$.subscribe((res) => {
            if (res && this.projectId != res?.id) {
                this.projectId = res?.id;
                this.getData();
            }
        });
    }

    protected exportData(): void {
        if (this.name == ReportTypes.AuditLog) {
            this.exportAuditLogData();
        }
        else if (this.name == ReportTypes.NoOfDPPADevices) {
            this.exportDPPADeviceData();
        }
        else if (this.name == ReportTypes.DuplicateDPNodeAddresses) {
            this.exportDPNodeData();
        }
        else if (this.name == ReportTypes.DuplicatePANodeAddresses) {
            this.exportPANodeData();
        }
        else if (this.name == ReportTypes.DuplicateRackSlotChannels) {
            this.exportDuplicateRackSlotChannelData();
        }
        else if (this.name == ReportTypes.PnIDException) {
            this.exportPnIdTagExceptionData();
        }
        else if (this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumber ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevision ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevisionInclNulls) {
            this.exportPnIdDeviceDocumentData();
        }
        else if (this.name == ReportTypes.InstrumentList) {
            this.exportInstrumentData();
        }
        else if (this.name == ReportTypes.NonInstrumentList) {
            this.exportNonInstrumentData();
        }
        else if (this.name == ReportTypes.OMItemInstrumentList) {
            this.exportOMItemInstrumentData();
        }
        else if (this.name == ReportTypes.SparesReport) {
            this.exportSparesListData();
        }
        else if (this.name == ReportTypes.SparesDetailReport) {
            this.exportSparesDetailsListData();
        }
        else if (this.name == ReportTypes.SparesReportPLC) {
            this.exportSparesPLCListData();
        }
        else if (this.name == ReportTypes.UnassociatedTags) {
            this.exportUnassociatedTagsData();
        }
        else if (this.name == ReportTypes.UnassociatedSkids || this.name == ReportTypes.UnassociatedJunctionBoxes ||
            this.name == ReportTypes.UnassociatedPanels || this.name == ReportTypes.UnassociatedStands) {
            this.exportUnassociatedData(this.name);
        }
        else if (this.name == ReportTypes.NatureOfSignalValidation) {
            this.exportNatureOfSignalData();
        }
        else if (this.name == ReportTypes.PSSTags) {
            this.exportPSSTagData();
        }
    }

    private exportAuditLogData() {
        const fileName = 'Export_AuditLogs';
        const auditLogData: ChangeLogInfoDtoModel[] = [];
        this.auditLogData.map((res) => {
            const info: ChangeLogInfoDtoModel = {
                context: res.key,
                createdDate: null,
                entityName: null,
                contextId: null,
                status: null,
                originalValues: null,
                newValues: null,
                createdBy: null,
            };
            auditLogData.push(info);
            res.items.map((item) => {
                auditLogData.push({
                    context: null,
                    createdDate: item.createdDate,
                    entityName: item.entityName,
                    contextId: item.contextId,
                    status: item.status,
                    originalValues: item.originalValues,
                    newValues: item.newValues,
                    createdBy: item.createdBy,
                });
            })
        })
        const columnMapping = {
            'context': "Context",
            'createdDate': 'Date',
            'entityName': 'Entity Name',
            'contextId': 'Entity Id',
            'status': 'Status',
            'originalValues': 'Old Values',
            'newValues': 'New Values',
            'createdBy': 'Modified By',
        };
        this._excelHelper.exportExcel(auditLogData ?? [], columnMapping, fileName);
    }

    private exportDPPADeviceData() {
        const fileName = 'Export_DPPADevices';
        const data: DPPADevicesDtoModel[] = [];
        this.dppaDeviceData.map((res) => {
            data.push(res);
            res.childInfo.map((subItem) => {
                data.push(subItem);
                subItem.childInfo.map((couplerItem) => {
                    data.push(couplerItem);
                    couplerItem.childInfo.map((mainItem) => {
                        data.push(mainItem);
                    });
                })
            })

        })
        const columnMapping = {
            'plcNumber': "PLC Number",
            'plcSlotNumber': 'PLC Slot Number',
            'dppaCoupler': 'DP Or PA Coupler',
            'afdHubNumber': 'AFD Hub Number',
            'noOfDPDevices': 'No Of DP Devices',
            'noOfPADevices': 'No Of PA Devices',
        };
        this._excelHelper.exportExcel(data ?? [], columnMapping, fileName);
    }

    private exportDPNodeData() {
        const fileName = 'Export_Duplicate_DP_Node_Addresses';
        const duplicateData: ViewInstrumentListLiveDtoModel[] = [];
        this.dpNodeData.map((res) => {
            const info = {} as ViewInstrumentListLiveDtoModel;
            for (const key in info) {
                if (info.hasOwnProperty(key)) {
                    info[key] = null;
                }
            }
            info.dpdpCoupler = res.dpdpCoupler;
            duplicateData.push(info);
            res.items.map((item) => {
                item.dpdpCoupler = null;
                duplicateData.push(item);
            })
        })
        const columnMapping = {
            'dpdpCoupler': "DPDP Coupler",
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "afdHubNumber": "AFD / Hub Number",
            "rackNo": "Rack No",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "dpNodeAddress": "DP Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportPANodeData() {
        const fileName = 'Export_Duplicate_PA_Node_Addresses';
        const duplicateData: ViewInstrumentListLiveDtoModel[] = [];
        this.paNodeData.map((res) => {
            const info = {} as ViewInstrumentListLiveDtoModel;
            for (const key in info) {
                if (info.hasOwnProperty(key)) {
                    info[key] = null;
                }
            }
            info.dppaCoupler = res.dppaCoupler;
            duplicateData.push(info);
            res.items.map((item) => {
                item.dppaCoupler = null;
                duplicateData.push(item);
            })
        })
        const columnMapping = {
            'dppaCoupler': "DPPA Coupler",
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "afdHubNumber": "AFD / Hub Number",
            "rackNo": "Rack No",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "paNodeAddress": "PA Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportDuplicateRackSlotChannelData() {
        const fileName = 'Export_Duplicate_Rack_Slot_Channels';
        const duplicateData: ViewInstrumentListLiveDtoModel[] = [];
        this.rackSlotChannelData.map((res) => {
            const info = {} as ViewInstrumentListLiveDtoModel;
            for (const key in info) {
                if (info.hasOwnProperty(key)) {
                    info[key] = null;
                }
            }
            info.rackNo = res.rackNo;
            duplicateData.push(info);
            res.items.map((item) => {
                item.rackNo = null;
                item.isActive = !item.isActive;
                duplicateData.push(item);
            })
        })
        const columnMapping = {
            'rackNo': "Rack No",
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "dpdpCoupler": "DPDP Coupler",
            "dppaCoupler": "DPPA Coupler",
            "afdHubNumber": "AFD / Hub Number",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "dpNodeAddress": "DP Node Address",
            "paNodeAddress": "PA Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
            "isActive": "Deleted",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportPnIdTagExceptionData() {
        const fileName = 'Export_PnIdTagExceptions';
        const duplicateData: ViewPnIDTagExceptionDtoModel[] = [];
        this.pnIdTagExceptionData.map((res) => {
            const info = {} as ViewPnIDTagExceptionDtoModel;
            for (const key in info) {
                if (info.hasOwnProperty(key)) {
                    info[key] = null;
                }
            }
            info.equipmentCode = res.key;
            duplicateData.push(info);
            res.items.map((item) => {
                item.equipmentCode = null;
                duplicateData.push(item);
            })
        })
        const columnMapping = {
            "equipmentCode": "Equipment Code",
            "tagName": "Tag",
            'processName': "Process",
            "subProcessName": "Sub Process",
            "streamName": "Stream",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "serviceDescription": "Service Description",
            "skidTag": "Skid Tag"
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportPnIdDeviceDocumentData() {
        const fileName = 'Export_PnIdDeviceDocumentReference';
        const duplicateData: ViewPnIDTagExceptionDtoModel[] = [];

        const columnMapping = {
            "tag": "Tag",
            "documentNumber": "Instrument/Control P&ID Document Number",
            'revision': "Instrument/Control P&ID Revision",
            "version": "Instrument/Control P&ID Version",
            "pnIdDocumentNumber": "Tag P&ID Document Number",
            "pnIdRevision": "Tag P&ID P&ID Revision",
            "pnIdVersion": "Tag P&ID P&ID Version",
        };
        this._excelHelper.exportExcel(this.pnIdDeviceDocumentData ?? [], columnMapping, fileName);
    }

    private exportInstrumentData() {
        const fileName = 'Export_InstrumentList';
        const duplicateData: ViewInstrumentListLiveDtoModel[] = this.instrumentListData.map((item) => ({
            ...item,
            isActive: !item.isActive
        }));
        const columnMapping = {
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "dpdpCoupler": "DPDP Coupler",
            "dppaCoupler": "DPPA Coupler",
            "afdHubNumber": "AFD / Hub Number",
            'rackNo': "Rack No",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "dpNodeAddress": "DP Node Address",
            "paNodeAddress": "PA Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
            "isActive": "Deleted",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportNonInstrumentData() {
        const fileName = 'Export_Non_InstrumentList';
        const duplicateData: ViewNonInstrumentListDtoModel[] = this.nonInstrumentListData.map((item) => ({
            ...item,
            isActive: !item.isActive
        }));
        const columnMapping = {
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "deviceType": "Device Type",
            "serviceDescription": "Service Description",
            "description": "Description",
            "dpNodeAddress": "DP Node Address",
            "noOfSlotsChannels": "No Of Slot Channels",
            "connectionParent": "Connection Parent",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "location": "Location",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "modelDescription": "Model Description",
            "architectureDrawing": "Architecture Drawing",
            "architectureDrawingSheet": "Architecture Drawing Sheet",
            "revision": "Revision",
            "revisionChanges": "Revision Changes",
            "isActive": "Deleted",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportOMItemInstrumentData() {
        const fileName = 'Export_OMItem_InstrumentList';
        const duplicateData: ViewOMItemInstrumentListDtoModel[] = this.omIteminstrumentListData.map((item) => ({
            ...item,
            isActive: !item.isActive
        }));
        const columnMapping = {
            'itemId': "ItemId",
            'itemDescription': "Item Description",
            'parentItemId': "Parent Item Id",
            'assetTypeId': "Asset Type Id",
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "dpdpCoupler": "DPDP Coupler",
            "dppaCoupler": "DPPA Coupler",
            "afdHubNumber": "AFD / Hub Number",
            'rackNo': "Rack No",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "dpNodeAddress": "DP Node Address",
            "paNodeAddress": "PA Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
            "historicalLogging": "Historical Logging",
            "historicalLoggingFrequency": "Historical Logging Frequency",
            "historicalLoggingResolution": "Historical Logging Resolution",
            "isActive": "Deleted",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportSparesListData() {
        const fileName = 'Export_SparesList';
        const data: SparesReportResponceDtoModel[] = [];
        this.sparesListData.map((res) => {
            data.push(res);
            res.childItems.map((subItem) => {
                data.push(subItem);
                subItem.childItems.map((couplerItem) => {
                    data.push(couplerItem);
                })
            })

        })
        const columnMapping = {
            'plcNumber': "PLC Number",
            'rack': "Rack",
            'natureOfSignal': "Nature Of Signal",
            'totalChanneles': "Total Channels",
            'usedChanneles': "Used Channels",
            'spareChannels': "Spare Channels",
            'percentUsed': "Percent Used",
            'percentSpare': "Percent Spare",
        };
        this._excelHelper.exportExcel(data ?? [], columnMapping, fileName);
    }

    private exportSparesDetailsListData() {
        const fileName = 'Export_SparesDetailsList';
        const data: SparesReportDetailsResponceDtoModel[] = [];
        this.sparesDetailsListData.map((res) => {
            data.push(res);
            res.childItems.map((subItem) => {
                data.push(subItem);
                subItem.childItems.map((couplerItem) => {
                    data.push(couplerItem);
                    couplerItem.childItems.map((mainItem) => {
                        data.push(mainItem);
                    });
                })
            })

        })
        const columnMapping = {
            'plcNumber': "PLC Number",
            'rack': "Rack",
            'natureOfSignal': "Nature Of Signal",
            'slotNumber': "Slot Number",
            'totalChanneles': "Total Channels",
            'usedChanneles': "Used Channels",
            'spareChannels': "Spare Channels",
            'percentUsed': "Percent Used",
            'percentSpare': "Percent Spare",
        };
        this._excelHelper.exportExcel(data ?? [], columnMapping, fileName);
    }

    private exportSparesPLCListData() {
        const fileName = 'Export_SparesPLCList';
        const data: SparesReportPLCResponceDtoModel[] = [];
        this.sparesPLCListData.map((res) => {
            data.push(res);
            res.childItems.map((subItem) => {
                data.push(subItem);
            })

        })
        const columnMapping = {
            'plcNumber': "PLC Number",
            'natureOfSignal': "Nature Of Signal",
            'totalChanneles': "Total Channels",
            'usedChanneles': "Used Channels",
            'spareChannels': "Spare Channels",
            'percentUsed': "Percent Used",
            'percentSpare': "Percent Spare",
        };
        this._excelHelper.exportExcel(data ?? [], columnMapping, fileName);
    }

    private exportUnassociatedTagsData(): void {
        const fileName = 'Export_Unassociated_Tags';
        const columnMapping = {
            'tag': "Tag",
            'process': "Process",
            'subProcess': "Sub Process",
            'stream': "Stream",
            'equipmentCode': "Equipment Code",
            'sequenceNumber': "Sequence Number",
            'equipmentIdentifier': "Equipment Identifier",
            'documentNumber': "Document Number",
            'revision': "Revision",
            'version': "Version",
        };
        this._excelHelper.exportExcel(this.tagListData ?? [], columnMapping, fileName);
    }

    private exportUnassociatedData(name: ReportTypes): void {
        let fileName = "";
        if (name == this.reportType.UnassociatedSkids)
            fileName = 'Export_Unassociated_Skids';

        if (name == this.reportType.UnassociatedJunctionBoxes)
            fileName = 'Export_Unassociated_JunctionBoxes';

        if (name == this.reportType.UnassociatedPanels)
            fileName = 'Export_Unassociated_Panels';

        const columnMapping = {
            'tagName': "Tag",
            'type': "Type",
            'area': "Area",
            'description': "Description",
            'documentNumber': "Document Number",
            'revision': "Revision",
            'version': "Version",
        };

        if (name == this.reportType.UnassociatedStands) {
            fileName = 'Export_Unassociated_Stands';
        }

        this._excelHelper.exportExcel(this.skidListData ?? [], columnMapping, fileName);
    }

    private exportNatureOfSignalData() {
        const fileName = 'Export_NatureOfSignalList';
        const duplicateData: ViewNatureOfSignalValidationFailuresDtoModel[] = this.natureListData.map((item) => ({
            ...item,
            isActive: !item.isActive
        }));
        const columnMapping = {
            'processNo': "Process No",
            "subProcess": "Sub Process",
            "streamName": "Stream",
            "equipmentCode": "Equipment Code",
            "sequenceNumber": "Sequence Number",
            "equipmentIdentifier": "Equipment Identifier",
            "tagName": "Tag",
            "instrumentParentTag": "Instr Parent Tag",
            "serviceDescription": "Service Description",
            "lineVesselNumber": "Line Vessel Number",
            "plant": "Plant",
            "area": "Area",
            "vendorSupply": "Vendor Supply",
            "skidNumber": "Skid Number",
            "standNumber": "Stand Number",
            "manufacturer": "Manufacturer",
            "modelNumber": "Model Number",
            "calibratedRangeMin": "Calibrated Range (Min)",
            "calibratedRangeMax": "Calibrated Range (Max)",
            "crUnits": "CR Units",
            "processRangeMin": "Process Range (Min)",
            "processRangeMax": "Process Range (Max)",
            "prUnits": "PR Units",
            "rlPosition": "RL / Position",
            "datasheetNumber": "Datasheet Number",
            "sheetNumber": "Sheet Number",
            "hookUpDrawing": "Hook Up Drawing",
            "terminationDiagram": "Termination Diagram",
            "pidNumber": "P&ID Number",
            "layoutDrawing": "Layout Drawing",
            "architecturalDrawing": "Architectural Drawing",
            "functionalDescriptionDocument": "Functional Description Document",
            "productProcurementNumber": "Product Procurement Number",
            "junctionBoxNumber": "Junction Box Number",
            "natureOfSignal": "Nature Of Signal",
            "failState": "Fail State",
            "gsdType": "GSD Type",
            "controlPanelNumber": "Control Panel Number",
            "plcNumber": "PLC Number",
            "plcSlotNumber": "PLC Slot Number",
            "fieldPanelNumber": "Field Panel Number",
            "dpdpCoupler": "DPDP Coupler",
            "dppaCoupler": "DPPA Coupler",
            "afdHubNumber": "AFD / Hub Number",
            'rackNo': "Rack No",
            "slotNo": "Slot No",
            "channelNo": "Channel No",
            "dpNodeAddress": "DP Node Address",
            "paNodeAddress": "PA Node Address",
            "revision": "Revision",
            "revisionChangesOutstandingComments": "Revision Changes / Outstanding Comments",
            "zone": "Zone",
            "bank": "Bank",
            "service": "Service",
            "variable": "Variable",
            "train": "Train",
            "workAreaPack": "Work Area Pack",
            "systemCode": "System Code",
            "subsystemCode": "Subsystem Code",
            "isInstrument": "Is Instrument",
            "isActive": "Deleted",
        };
        this._excelHelper.exportExcel(duplicateData ?? [], columnMapping, fileName);
    }

    private exportPSSTagData() {
        const fileName = 'Export_PSSTags';
        const columnMapping = {
            'kind': "Kind",
            'cbTagNumber': "CB Tag Number",
            'cbVariableType': "CB Variable Type",
            'plcTag': "PLC Tag",
            'pcS7VariableType': "PCS7 Variable Type",
            'signalExtension': "Signal Extension",
            'processName': "Process",
            'subProcessName': "Sub Process",
            'streamName': "Stream",
            'equipmentCode': "Equipment Code",
            'sequenceNumber': "Sequence Number",
            'equipmentIdentifier': "Equipment Identifier",
            'tagName': "Tag",
            'plcNumber': "PLC Number",
            'natureOfSignalName': "Nature Of Signal",
            'gsdType': "GSD Type",
            'manufacturer': "Manufacturer",
            'model': "Model",
        };
        this._excelHelper.exportExcel(this.pssTagListData ?? [], columnMapping, fileName);
    }

    private getData(): void {
        if (this.name == ReportTypes.AuditLog) {
            this.getAuditLogData();
        }
        else if (this.name == ReportTypes.NoOfDPPADevices) {
            this.getDPPADeviceData();
        }
        else if (this.name == ReportTypes.DuplicateDPNodeAddresses) {
            this.getDuplicateDPNodeData();
        }
        else if (this.name == ReportTypes.DuplicatePANodeAddresses) {
            this.getDuplicatePANodeData();
        }
        else if (this.name == ReportTypes.DuplicateRackSlotChannels) {
            this.getDuplicateRackSlotChannelData();
        }
        else if (this.name == ReportTypes.PnIDException) {
            this.getPnIdTgaExceptionData();
        }
        else if (this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumber ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevision ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevisionInclNulls) {
            const type: PnIdDeviceMisMatchDocumentReference = PnIdDeviceMisMatchDocumentReference[this.name];
            this.getPnIdDeviceDocumentReferenceData(type);
        }
        else if (this.name == ReportTypes.InstrumentList) {
            this.getInstrumentList();
        }
        else if (this.name == ReportTypes.NonInstrumentList) {
            this.getNonInstrumentList();
        }
        else if (this.name == ReportTypes.OMItemInstrumentList) {
            this.getOMItemInstrumentList();
        }
        else if (this.name == ReportTypes.SparesReport) {
            this.getSparesList();
        }
        else if (this.name == ReportTypes.SparesDetailReport) {
            this.getSparesDetailsList();
        }
        else if (this.name == ReportTypes.SparesReportPLC) {
            this.getSparesPLCList();
        }
        else if (this.name == ReportTypes.UnassociatedTags) {
            this.getUnassociatedTags();
        }
        else if (this.name == ReportTypes.UnassociatedSkids) {
            this.getUnassociatedSkids();
        }
        else if (this.name == ReportTypes.UnassociatedStands) {
            this.getUnassociatedStands();
        }
        else if (this.name == ReportTypes.UnassociatedJunctionBoxes) {
            this.getUnassociatedJunctionBoxes();
        }
        else if (this.name == ReportTypes.UnassociatedPanels) {
            this.getUnassociatedPanels();
        }
        else if (this.name == ReportTypes.NatureOfSignalValidation) {
            this.getNatureOfSignalData();
        }
        else if (this.name == ReportTypes.PSSTags) {
            this.getPSSTagsData();
        }
    }

    private getAuditLogData(): void {
        this._reportService.getAuditLogData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.auditLogData = res;
                this.auditLogReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getDPPADeviceData(): void {
        this._reportService.getDPPADeviceData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.dppaDeviceData = res;
                this.dppaDevicesReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getDuplicateDPNodeData(): void {
        this._reportService.getDuplicateDPNodeData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.dpNodeData = res;
                this.dpNodeReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getDuplicatePANodeData(): void {
        this._reportService.getDuplicatePANodeData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.paNodeData = res;
                this.paNodeReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getDuplicateRackSlotChannelData(): void {
        this._reportService.getDuplicateRackSlotChannelData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.rackSlotChannelData = res;
                this.rackSlotReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getPnIdTgaExceptionData(): void {
        this._reportService.getPnIdExceptionData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.pnIdTagExceptionData = res;
                this.pnIdTagReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getPnIdDeviceDocumentReferenceData(type: PnIdDeviceMisMatchDocumentReference): void {
        const info: PnIdDeviceDocumentReferenceRequestDtoModel = {
            projectId: this.projectId,
            type: type
        };
        this._reportService.getPnIdDeviceDocumentReferenceData(info)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.pnIdDeviceDocumentData = res;
                this.pnIdDocumentReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getInstrumentList(): void {
        this._reportService.getInstrumentListData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.instrumentListData = res;
                this.instrumentReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getNonInstrumentList(): void {
        this._reportService.getNonInstrumentListData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.nonInstrumentListData = res;
                this.nonInstrumentReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getOMItemInstrumentList(): void {
        this._reportService.getOMItemInstrumentListData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.omIteminstrumentListData = res;
                this.omInstrumentReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getSparesList(): void {
        this._reportService.getSparesData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.sparesListData = res;
                this.sparesReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getSparesDetailsList(): void {
        this._reportService.getSparesDetailsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.sparesDetailsListData = res;
                this.sparesDetailReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getSparesPLCList(): void {
        this._reportService.getSparesPLCData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.sparesPLCListData = res;
                this.sparesPLCReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getUnassociatedTags(): void {
        this._reportService.getUnassociatedTagsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.tagListData = res;
                this.tagsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getUnassociatedSkids(): void {
        this._reportService.getUnassociatedSkidsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.skidListData = res;
                this.skidsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getUnassociatedStands(): void {
        this._reportService.getUnassociatedStandsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.skidsReport.isStand = true;
                this.skidListData = res;
                this.skidsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getUnassociatedJunctionBoxes(): void {
        this._reportService.getUnassociatedJunctionBoxesData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.skidListData = res;
                this.skidsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getUnassociatedPanels(): void {
        this._reportService.getUnassociatedPanelsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.skidListData = res;
                this.skidsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getNatureOfSignalData(): void {
        this._reportService.getNatureOfSignalsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.natureListData = res;
                this.natureOfSignalReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getPSSTagsData(): void {
        this._reportService.getPSSTagsData(this.projectId)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.pssTagListData = res;
                this.pssTagsReport.item = res;
                this.isExistRecords = res.length != 0 ? true : false;
                this._cdr.detectChanges();
            })
    }

    private getTitle(): void {
        if (this.name == ReportTypes.AuditLog) {
            this.title = "Audit Log";
        }
        else if (this.name == ReportTypes.NoOfDPPADevices) {
            this.title = "Number of DP and PA Devices";
        }
        else if (this.name == ReportTypes.DuplicateDPNodeAddresses) {
            this.title = "Duplicate DP Node Addresses";
        }
        else if (this.name == ReportTypes.DuplicatePANodeAddresses) {
            this.title = "Duplicate PA Node Addresses";
        }
        else if (this.name == ReportTypes.DuplicateRackSlotChannels) {
            this.title = "Duplicate Rack Slot Channels";
        }
        else if (this.name == ReportTypes.PnIDException) {
            this.title = "P&ID Tag Exception";
        }
        else if (this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumber ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevision ||
            this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumberVersionRevisionInclNulls) {
            this.title = "I&C-P&ID Document Reference Exception";
        }
        else if (this.name == ReportTypes.InstrumentList) {
            this.title = "Instrument List";
        }
        else if (this.name == ReportTypes.NonInstrumentList) {
            this.title = "Non-Instrument List";
        }
        else if (this.name == ReportTypes.OMItemInstrumentList) {
            this.title = "O&M Item Instrument List";
        }
        else if (this.name == ReportTypes.SparesReport) {
            this.title = "Spares";
        }
        else if (this.name == ReportTypes.SparesDetailReport) {
            this.title = "PLC Spare I/O - 1";
        }
        else if (this.name == ReportTypes.SparesReportPLC) {
            this.title = "PLC Spare I/O - 2";
        }
        else if (this.name == ReportTypes.UnassociatedTags) {
            this.title = "Unassociated Tags";
        }
        else if (this.name == ReportTypes.UnassociatedSkids) {
            this.title = "Unassociated Skids";
        }
        else if (this.name == ReportTypes.UnassociatedStands) {
            this.title = "Unassociated Stands";
        }
        else if (this.name == ReportTypes.UnassociatedJunctionBoxes) {
            this.title = "Unassociated Junction Boxes";
        }
        else if (this.name == ReportTypes.UnassociatedPanels) {
            this.title = "Unassociated Panels";
        }
        else if (this.name == ReportTypes.NatureOfSignalValidation) {
            this.title = "Nature Of Signal Validations";
        }
        else if (this.name == ReportTypes.PSSTags) {
            this.title = "PSS Tags";
        }

        this.subTitle = this.findRecordByName(this.name)?.description ?? "";
        if (this.name == ReportTypes.PnIDDeviceMismatchedDocumentNumber) {
            this.subTitle += " <br/><b>Note :</b> (Revision/version are not checked)"
        }
    }

    private findRecordByName(nameToFind: string): ReportInfoDtoModel | undefined {
        const flattenedItems = this.reportList.reduce((acc, report) => {
            return acc.concat(report.items || []);
        }, [] as ReportInfoDtoModel[]);

        return flattenedItems.find(report => report.url === nameToFind);
    }


    private getAllReports(): void {
        this._reportService.getReportList()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.reportList = res;
                this._cdr.detectChanges();
                this.getTitle();
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}