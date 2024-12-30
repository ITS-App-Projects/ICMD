import { ReferenceDocumentInfoDtoModel } from '@c/masters/reference-document/list-reference-document-table';
import {
  DropdownInfoDtoModel,
  KeyValueInfoDtoModel
} from '@m/common';

export interface DeviceDropdownInfoDtoModel {
    deviceTypes: DropdownInfoDtoModel[] | null;
    manufacturerList: DropdownInfoDtoModel[] | null;
    failStateList: DropdownInfoDtoModel[] | null;
    zoneList: DropdownInfoDtoModel[] | null;
    bankList: DropdownInfoDtoModel[] | null;
    trainList: DropdownInfoDtoModel[] | null;
    natureOfSignalList: DropdownInfoDtoModel[] | null;
    fieldPanelList: DropdownInfoDtoModel[] | null;
    skidList: DropdownInfoDtoModel[] | null;
    standList: DropdownInfoDtoModel[] | null;
    junctionBoxList: DropdownInfoDtoModel[] | null;
    isInstrumentOptionsList: KeyValueInfoDtoModel[] | null;
    referenceDocTypeList: DropdownInfoDtoModel[] | null;
    workAreaPackList: DropdownInfoDtoModel[] | null;
    connectionTagList: DropdownInfoDtoModel[] | null;
    instrumentTagList: DropdownInfoDtoModel[] | null;
    tagList: DropdownInfoDtoModel[] | null;
    cableDeviceTagList: DropdownInfoDtoModel[] | null;
}

export interface CreateOrEditDeviceDtoModel {
    id: string;
    deviceTypeId: string;
    tagId: string;
    projectId: string;
    isInstrument: string;
    manufacturerId: string | null;
    deviceModelId: string | null;
    connectionParentTagId: string | null;
    instrumentParentTagId: string | null;
    lineVesselNumber: string | null;
    vendorSupply: boolean | null;
    failStateId: string | null;
    serviceZoneId: string | null;
    serviceBankId: string | null;
    service: string | null;
    serviceTrainId: string | null;
    variable: string;
    natureOfSignalId: string | null;
    serviceDescription: string | null;
    skidTagId: string | null;
    panelTagId: string | null;
    junctionBoxTagId: string | null;
    standTagId: string | null;
    serialNumber: string | null;
    historicalLogging: boolean | null;
    historicalLoggingFrequency: number | null;
    historicalLoggingResolution: number | null;
    revisionChanges: string | null;
    workAreaPackId: string | null;
    systemId: string | null;
    subSystemId: string | null;
    referenceDocumentIds: string[];
    selectedReferenceDocId: string | null;
    selectReferenceDocTypeId: string | null;
    referenceDocumentInfo: ReferenceDocumentInfoDtoModel[] | null;
    attributes: AttributeValueDtoModel[] | null;
    connectionCableTagId: string | null;
    instrumentCableTagId: string | null;
}


export interface AttributeValueDtoModel {
    id: string;
    name: string | null;
    valueType: string | null;
    required: boolean;
    value: string | null;
}

export interface DeviceAttributeInfoDtoModel {
    deviceTypeId: string | null;
    deviceModelId: string | null;
    natureOfSignalId: string | null;
    projectId: string | null;
    deviceId: string | null;
    connectionParentTagId: string | null;
}