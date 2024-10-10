import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ChangeLogListDtoModel } from "@c/manage-reports/view-auditLog-report";
import { DPPADevicesDtoModel } from "@c/manage-reports/view-dppa-devices-report";
import { DuplicateDPNodeAddressDtoModel, ViewInstrumentListLiveDtoModel } from "@c/manage-reports/view-duplicate-dpnode-report";
import { DuplicatePANodeAddressDtoModel } from "@c/manage-reports/view-duplicate-panode-report";
import { DuplicateRackSlotChannelDtoModel } from "@c/manage-reports/view-duplicate-rack-slot-report";
import { ViewNatureOfSignalValidationFailuresDtoModel } from "@c/manage-reports/view-nature-of-signal-validations-report";
import { ViewOMItemInstrumentListDtoModel } from "@c/manage-reports/view-om-item-instrument-report";
import { PnIdDeviceDocumentReferenceRequestDtoModel, ViewPnIDDeviceDocumentReferenceCompareDtoModel } from "@c/manage-reports/view-pnId-device-documentReference-report";
import { PnIDTagExceptionInfoDtoModel } from "@c/manage-reports/view-pnId-tag-exception-report";
import { ViewPSSTagsDtoModel } from "@c/manage-reports/view-pss-tags-report";
import { SparesReportDetailsResponceDtoModel } from "@c/manage-reports/view-spares-detail-report";
import { SparesReportPLCResponceDtoModel } from "@c/manage-reports/view-spares-plc-report";
import { SparesReportResponceDtoModel } from "@c/manage-reports/view-spares-report";
import { ViewUnassociatedSkidsDto } from "@c/manage-reports/view-unassociated-skids-report";
import { UnassociatedTagsDtoModel } from "@c/manage-reports/view-unassociated-tags-report";
import { ViewNonInstrumentListDtoModel } from "@c/nonInstrument-list/list-nonInstrument-table";
import { environment } from "@env/environment";
import { ReportListDtoModel } from "@p/admin/manage-reports/list-reports-page";
import { Observable } from "rxjs";

@Injectable()
export class ReportService {
    constructor(private _http: HttpClient) { }

    public getReportList(): Observable<ReportListDtoModel[]> {
        return this._http.get<ReportListDtoModel[]>(
            `${environment.apiUrl}Report/GetReportList`
        );
    }

    public getAuditLogData(projectId: string): Observable<ChangeLogListDtoModel[]> {
        return this._http.get<ChangeLogListDtoModel[]>(
            `${environment.apiUrl}Report/GetAuditLogData?projectId=${projectId}`
        );
    }

    public getDPPADeviceData(projectId: string): Observable<DPPADevicesDtoModel[]> {
        return this._http.get<DPPADevicesDtoModel[]>(
            `${environment.apiUrl}Report/GetDPPADevicesData?projectId=${projectId}`
        );
    }

    public getDuplicateDPNodeData(projectId: string): Observable<DuplicateDPNodeAddressDtoModel[]> {
        return this._http.get<DuplicateDPNodeAddressDtoModel[]>(
            `${environment.apiUrl}Report/GetDuplicateDPNodeAddress?projectId=${projectId}`
        );
    }

    public getDuplicatePANodeData(projectId: string): Observable<DuplicatePANodeAddressDtoModel[]> {
        return this._http.get<DuplicatePANodeAddressDtoModel[]>(
            `${environment.apiUrl}Report/GetDuplicatePANodeAddress?projectId=${projectId}`
        );
    }

    public getDuplicateRackSlotChannelData(projectId: string): Observable<DuplicateRackSlotChannelDtoModel[]> {
        return this._http.get<DuplicateRackSlotChannelDtoModel[]>(
            `${environment.apiUrl}Report/GetDuplicateRackSlotChannelData?projectId=${projectId}`
        );
    }

    public getPnIdExceptionData(projectId: string): Observable<PnIDTagExceptionInfoDtoModel[]> {
        return this._http.get<PnIDTagExceptionInfoDtoModel[]>(
            `${environment.apiUrl}Report/GetPnIdTagExceptionData?projectId=${projectId}`
        );
    }

    public getPnIdDeviceDocumentReferenceData(info: PnIdDeviceDocumentReferenceRequestDtoModel): Observable<ViewPnIDDeviceDocumentReferenceCompareDtoModel[]> {
        return this._http.post<ViewPnIDDeviceDocumentReferenceCompareDtoModel[]>(
            `${environment.apiUrl}Report/GetPnIdMisMatchedDocumentReferenceData`, info
        );
    }

    public getInstrumentListData(projectId: string): Observable<ViewInstrumentListLiveDtoModel[]> {
        return this._http.get<ViewInstrumentListLiveDtoModel[]>(
            `${environment.apiUrl}Report/GetInstrumentListData?projectId=${projectId}`
        );
    }

    public getNonInstrumentListData(projectId: string): Observable<ViewNonInstrumentListDtoModel[]> {
        return this._http.get<ViewNonInstrumentListDtoModel[]>(
            `${environment.apiUrl}Report/GetNonInstrumentListData?projectId=${projectId}`
        );
    }

    public getOMItemInstrumentListData(projectId: string): Observable<ViewOMItemInstrumentListDtoModel[]> {
        return this._http.get<ViewOMItemInstrumentListDtoModel[]>(
            `${environment.apiUrl}Report/GetOMItemInstrumentListData?projectId=${projectId}`
        );
    }

    public getSparesData(projectId: string): Observable<SparesReportResponceDtoModel[]> {
        return this._http.get<SparesReportResponceDtoModel[]>(
            `${environment.apiUrl}Report/GetSparesData?projectId=${projectId}`
        );
    }

    public getSparesDetailsData(projectId: string): Observable<SparesReportDetailsResponceDtoModel[]> {
        return this._http.get<SparesReportDetailsResponceDtoModel[]>(
            `${environment.apiUrl}Report/GetSparesDetailsData?projectId=${projectId}`
        );
    }

    public getSparesPLCData(projectId: string): Observable<SparesReportPLCResponceDtoModel[]> {
        return this._http.get<SparesReportPLCResponceDtoModel[]>(
            `${environment.apiUrl}Report/GetSparesPLCData?projectId=${projectId}`
        );
    }

    public getUnassociatedTagsData(projectId: string): Observable<UnassociatedTagsDtoModel[]> {
        return this._http.get<UnassociatedTagsDtoModel[]>(
            `${environment.apiUrl}Report/GetUnassociatedTagsData?projectId=${projectId}`
        );
    }

    public getUnassociatedSkidsData(projectId: string): Observable<ViewUnassociatedSkidsDto[]> {
        return this._http.get<ViewUnassociatedSkidsDto[]>(
            `${environment.apiUrl}Report/GetUnassociatedSkidsData?projectId=${projectId}`
        );
    }

    public getUnassociatedStandsData(projectId: string): Observable<ViewUnassociatedSkidsDto[]> {
        return this._http.get<ViewUnassociatedSkidsDto[]>(
            `${environment.apiUrl}Report/GetUnassociatedStandsData?projectId=${projectId}`
        );
    }

    public getUnassociatedJunctionBoxesData(projectId: string): Observable<ViewUnassociatedSkidsDto[]> {
        return this._http.get<ViewUnassociatedSkidsDto[]>(
            `${environment.apiUrl}Report/GetUnassociatedJunctionBoxesData?projectId=${projectId}`
        );
    }

    public getUnassociatedPanelsData(projectId: string): Observable<ViewUnassociatedSkidsDto[]> {
        return this._http.get<ViewUnassociatedSkidsDto[]>(
            `${environment.apiUrl}Report/GetUnassociatedPanelsData?projectId=${projectId}`
        );
    }

    public getNatureOfSignalsData(projectId: string): Observable<ViewNatureOfSignalValidationFailuresDtoModel[]> {
        return this._http.get<ViewNatureOfSignalValidationFailuresDtoModel[]>(
            `${environment.apiUrl}Report/GetNatureOfSignalValidationsData?projectId=${projectId}`
        );
    }

    public getPSSTagsData(projectId: string): Observable<ViewPSSTagsDtoModel[]> {
        return this._http.get<ViewPSSTagsDtoModel[]>(
            `${environment.apiUrl}Report/GetPSSTagsData?projectId=${projectId}`
        );
    }
}