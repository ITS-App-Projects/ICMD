export interface ViewNatureOfSignalValidationFailuresDtoModel {
    deviceId: string | null;
    processNo: string | null;
    subProcess: string | null;
    streamName: string | null;
    equipmentCode: string | null;
    sequenceNumber: string | null;
    equipmentIdentifier: string | null;
    tagName: string | null;
    instrumentParentTag: string | null;
    serviceDescription: string | null;
    lineVesselNumber: string | null;
    plant: number | null;
    area: number | null;
    vendorSupply: boolean | null;
    skidNumber: string | null;
    standNumber: string | null;
    manufacturer: string | null;
    modelNumber: string | null;
    calibratedRangeMin: string | null;
    calibratedRangeMax: string | null;
    crUnits: string | null;
    processRangeMin: string | null;
    processRangeMax: string | null;
    prUnits: string | null;
    rlPosition: string | null;
    datasheetNumber: string | null;
    sheetNumber: string | null;
    hookUpDrawing: string | null;
    terminationDiagram: string | null;
    pidNumber: string | null;
    layoutDrawing: string | null;
    architecturalDrawing: string | null;
    functionalDescriptionDocument: string | null;
    productProcurementNumber: string | null;
    junctionBoxNumber: string | null;
    natureOfSignal: string | null;
    failState: string | null;
    gsdType: string | null;
    controlPanelNumber: string | null;
    plcNumber: string | null;
    plcSlotNumber: string | null;
    fieldPanelNumber: string | null;
    dpdpCoupler: string | null;
    dppaCoupler: string | null;
    afdHubNumber: string | null;
    rackNo: string | null;
    slotNo: string | null;
    channelNo: string | null;
    dpNodeAddress: string | null;
    paNodeAddress: string | null;
    revision: number | null;
    revisionChangesOutstandingComments: string | null;
    zone: string | null;
    bank: string | null;
    service: string | null;
    variable: string | null;
    train: string | null;
    workAreaPack: string | null;
    systemCode: string | null;
    subsystemCode: string | null;
    historicalLogging: boolean | null;
    historicalLoggingFrequency: number | null;
    historicalLoggingResolution: number | null;
    isInstrument:string | null;
    projectId: string | null;
    isActive: boolean;
    isDeleted: boolean;
}