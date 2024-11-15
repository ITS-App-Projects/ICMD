export interface ViewNonInstrumentListDtoModel {
    deviceId: string | null;
    processNo: string | null;
    subProcess: string | null;
    streamName: string | null;
    equipmentCode: string | null;
    sequenceNumber: string | null;
    equipmentIdentifier: string | null;
    tagName: string | null;
    deviceType: string | null;
    isInstrument: string | null;
    connectionParentTag: string | null;
    instrumentParentTag: string | null;
    serviceDescription: string | null;
    description: string | null;
    natureOfSignal: string | null;
    dpNodeAddress: number | null;
    noOfSlotsChannels: number | null;
    slotNumber: number | null;
    plcNumber: string | null;
    plcSlotNumber: number | null;
    location: string | null;
    manufacturer: string | null;
    modelNumber: string | null;
    modelDescription: string | null;
    architectureDrawing: string | null;
    architectureDrawingSheet: string | null;
    revision: number | null;
    revisionChanges: string | null;
    projectId: string | null;
    isDeleted: boolean;
    isActive: boolean;
}