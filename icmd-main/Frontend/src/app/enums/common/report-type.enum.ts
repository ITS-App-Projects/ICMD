export enum ReportTypes {
    AuditLog = "AuditLog",
    NoOfDPPADevices = "NoOfDPPADevices",
    DuplicateDPNodeAddresses = "DuplicateDPNodeAddresses",
    DuplicatePANodeAddresses = "DuplicatePANodeAddresses",
    DuplicateRackSlotChannels = "DuplicateRackSlotChannels",
    PnIDException = "PnIDException",
    PnIDDeviceMismatchedDocumentNumber = "PnID_Device_MismatchedDocumentNumber",
    PnIDDeviceMismatchedDocumentNumberVersionRevision = "PnID_Device_MismatchedDocumentNumber_VersionRevision",
    PnIDDeviceMismatchedDocumentNumberVersionRevisionInclNulls = "PnID_Device_MismatchedDocumentNumber_VersionRevisionInclNulls",
    InstrumentList = "InstrumentList",
    NonInstrumentList = "NonInstrumentList",
    OMItemInstrumentList = "OMItemInstrumentList",
    SparesReport = "SparesReport",
    SparesDetailReport = "SparesReportDetailed",
    SparesReportPLC = "SparesReportPLC",
    UnassociatedTags = "UnassociatedTags",
    UnassociatedSkids = "UnassociatedSkids",
    UnassociatedStands = "UnassociatedStands",
    UnassociatedJunctionBoxes = "UnassociatedJunctionBoxes",
    UnassociatedPanels = "UnassociatedPanels",
    NatureOfSignalValidation = "NatureOfSignalValidation",
    PSSTags = "PSSTags"
}