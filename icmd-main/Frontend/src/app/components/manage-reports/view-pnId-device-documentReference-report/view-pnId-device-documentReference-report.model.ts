import { PnIdDeviceMisMatchDocumentReference } from "@e/common";

export interface ViewPnIDDeviceDocumentReferenceCompareDtoModel {
    projectId: string;
    deviceId: string;
    tag: string | null;
    documentNumber: string | null;
    revision: string | null;
    version: string | null;
    pnIdDocumentNumber: string | null;
    pnIdRevision: string | null;
    pnIdVersion: string | null;
}

export interface PnIdDeviceDocumentReferenceRequestDtoModel {
    projectId: string;
    type: PnIdDeviceMisMatchDocumentReference;
}