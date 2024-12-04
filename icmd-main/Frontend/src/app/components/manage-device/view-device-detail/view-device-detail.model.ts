import { ReferenceDocumentInfoDtoModel } from '@c/masters/reference-document/list-reference-document-table';

import { AttributeValueDtoModel } from '../create-edit-device-form';

export interface ViewDeviceInfoDtoModel {
    id: string;
    deviceType: string | null;
    tag: string | null;
    isInstrument: string | null;
    manufacturer: string | null;
    deviceModel: string | null;
    lineVesselNumber: string | null;
    vendorSupply: boolean | null;
    failState: string | null;
    area: number | null;
    serviceZone: string | null;
    serviceBank: string | null;
    service: string | null;
    serviceTrain: string | null;
    variable: string | null;
    natureOfSignal: string | null;
    serviceDescription: string | null;
    skidTag: string | null;
    panelTag: string | null;
    junctionBoxTag: string | null;
    standTag: string | null;
    serialNumber: string | null;
    historicalLogging: boolean | null;
    historicalLoggingFrequency: number | null;
    historicalLoggingResolution: number | null;
    revisionChanges: string | null;
    workAreaPack: string | null;
    system: string | null;
    subSystem: string | null;
    referenceDocumentInfo: ReferenceDocumentInfoDtoModel[] | null;
    attributes: AttributeValueDtoModel[] | null;
    status: string | null;
    processName: string | null;
    subProcessName: string | null;
    streamName: string | null;
    equipmentCode: string | null;
    sequenceNumber: string | null;
    revision: number | null;
    equipmentIdentifier: string | null;
    connectionParentTag: string | null;
    instrumentParentTag: string | null;
    originCableTag: string | null;
    destinationCableTag: string | null;
}