export const decodedTokenConstants = {
  userId: "UserId",
  userFullName: "UserFullName",
  email: "Email",
  userName: "UserName",
  roleName: "RoleName"
};

//#region For Export
export const manangeUserListTableColumns = [
  { key: 'fullName', label: 'Name' },
  { key: 'userName', label: 'User Name' },
  { key: 'roleName', label: 'Role' },
  { key: 'email', label: "Email" },
  { key: 'phoneNumber', label: 'Phone No.' },
  { key: 'actions', label: 'Actions' },
];

export const manageProjectListTableColumns = [
  { key: 'name', label: 'Name' },
  { key: 'description', label: 'Description' },
  { key: 'isActive', label: 'Status' },
  { key: 'actions', label: 'Actions' },
];

export const masterWorkAreaListTableColumn = [
  { key: 'number', label: 'Number' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterZoneListTableColumn = [
  { key: 'zone', label: 'Zone' },
  { key: 'description', label: 'Description' },
  { key: 'area', label: 'Area' },
  { key: 'actions', label: 'Actions' },
];

export const masterSystemListTableColumn = [
  { key: 'number', label: 'Number' },
  { key: 'description', label: 'Description' },
  { key: 'workAreaPack', label: 'Work Area Pack' },
  { key: 'actions', label: 'Actions' },
];

export const masterSubSystemListTableColumn = [
  { key: 'workAreaPack', label: 'Work Area Pack' },
  { key: 'system', label: 'System' },
  { key: "number", label: "Number" },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterProcessListTableColumn = [
  { key: 'processName', label: 'Process Name' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterSubProcessListTableColumn = [
  { key: 'subProcessName', label: 'Sub-Process Name' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterStreamListTableColumn = [
  { key: 'streamName', label: 'Stream Name' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterReferenceDocumentListTableColumn = [
  { key: 'documentNumber', label: 'Document No' },
  { key: 'referenceDocumentType', label: 'Type' },
  { key: 'url', label: 'URL' },
  { key: 'description', label: 'Description' },
  { key: 'version', label: "Version" },
  { key: 'revision', label: 'Revision' },
  { key: 'date', label: 'Date' },
  { key: 'sheet', label: 'Sheet' },
  { key: 'actions', label: 'Actions' },
];

export const masterTagListTableColumn = [
  { key: 'tag', label: 'Tag' },
  { key: 'field1String', label: 'Process' },
  { key: 'field2String', label: 'Sub Process' },
  { key: 'field3String', label: 'Stream' },
  { key: 'field4String', label: 'Equipment Code' },
  { key: 'field5String', label: 'Sequence Number' },
  { key: 'field6String', label: 'Equipment Identifier' },
  { key: 'actions', label: 'Actions' },
];

export const masterJunctionBoxListTableColumn = [
  { key: 'tag', label: 'Tag' },
  { key: 'process', label: 'Process No' },
  { key: 'subProcess', label: "Sub Process" },
  { key: 'stream', label: "Stream" },
  { key: 'equipmentCode', label: "Equipment Code" },
  { key: 'sequenceNumber', label: "Sequence Number" },
  { key: 'equipmentIdentifier', label: "Equipment Identifier" },
  { key: 'type', label: "Type" },
  { key: 'description', label: "Description" },
  { key: 'referenceDocumentType', label: "Reference Document Type" },
  { key: 'documentNumber', label: "Reference Document" },
  { key: 'actions', label: 'Actions' },
];

export const masterStandListTableColumn = [
  { key: 'tag', label: 'Tag' },
  { key: 'process', label: 'Process No' },
  { key: 'subProcess', label: "Sub Process" },
  { key: 'stream', label: "Stream" },
  { key: 'equipmentCode', label: "Equipment Code" },
  { key: 'sequenceNumber', label: "Sequence Number" },
  { key: 'equipmentIdentifier', label: "Equipment Identifier" },
  { key: 'description', label: "Description" },
  { key: 'type', label: "Type" },
  { key: 'area', label: "Area" },
  { key: 'referenceDocumentType', label: "Reference Document Type" },
  { key: 'documentNumber', label: "Reference Document" },
  { key: 'actions', label: 'Actions' },
];

export const masterEquipmentCodeListTableColumn = [
  { key: 'code', label: 'Code' },
  { key: 'descriptor', label: 'Descriptor' },
  { key: 'actions', label: 'Actions' }
];

export const masterTagTypeListTableColumn = [
  { key: 'name', label: 'Name' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' },
];

export const masterTagDescriptorListTableColumn = [
  { key: 'name', label: 'Name' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' }
];

export const masterManufacturerListTableColumn = [
  { key: 'name', label: 'Name' },
  { key: 'description', label: 'Description' },
  { key: 'comment', label: 'Comment' },
  { key: 'actions', label: 'Actions' }
];

export const masterDeviceListTableColumn = [
  { key: 'model', label: 'Model' },
  { key: 'description', label: 'Description' },
  { key: 'manufacturer', label: 'Manufacturer' },
  { key: 'actions', label: 'Actions' }
];

export const masterDeviceTypeListTableColumn = [
  { key: 'type', label: 'Type' },
  { key: 'description', label: 'Description' },
  { key: 'actions', label: 'Actions' }
];

export const instrumentListTableColumns = [
  { key: 'processNo', label: 'Process Number' },
  { key: 'subProcess', label: 'Sub Process' },
  { key: 'streamName', label: 'Stream Name' },
  { key: 'equipmentCode', label: 'Equipment Code' },
  { key: 'sequenceNumber', label: 'Sequence Number' },
  { key: 'equipmentIdentifier', label: 'Equipment Identifier' },
  { key: 'tagName', label: 'Tag' },
  { key: 'deviceType', label: 'Device Type' },
  { key: 'isInstrument', label: 'Is Instrument' },
  { key: 'connectionParentTag', label: 'Connection Parent Tag' },
  { key: 'instrumentParentTag', label: 'Instrument Parent Tag' },
  { key: 'serviceDescription', label: 'Service Description' },
  { key: 'lineVesselNumber', label: 'Line / Vessel Number' },
  { key: 'plant', label: 'Plant' },
  { key: 'area', label: 'Area' },
  { key: 'vendorSupply', label: 'Vendor Supply' },
  { key: 'skidNumber', label: 'Skid Number' },
  { key: 'standNumber', label: 'Stand Number' },
  { key: 'manufacturer', label: 'Manufacturer' },
  { key: 'modelNumber', label: 'Model Number' },
  { key: 'calibratedRangeMin', label: 'Calibrated Range (Min)' },
  { key: 'calibratedRangeMax', label: 'Calibrated Range (Max)' },
  { key: 'crUnits', label: 'CR Units' },
  { key: 'processRangeMin', label: 'Process Range (Min)' },
  { key: 'processRangeMax', label: 'Process Range (Max)' },
  { key: 'prUnits', label: 'PR Units' },
  { key: 'rlPosition', label: 'RL / Position' },
  { key: 'datasheetNumber', label: 'Datasheet Number' },
  { key: 'sheetNumber', label: 'Sheet Number' },
  { key: 'hookUpDrawing', label: 'Hook Up Drawing' },
  { key: 'terminationDiagram', label: 'Termination Diagram' },
  { key: 'pidNumber', label: 'P&ID Number' },
  { key: 'layoutDrawing', label: 'Layout Drawing' },
  { key: 'architecturalDrawing', label: 'Architectural Drawing' },
  { key: 'junctionBoxNumber', label: 'Junction Box Number' },
  { key: 'natureOfSignal', label: 'Nature Of Signal' },
  { key: 'failState', label: 'Fail State' },
  { key: 'gsdType', label: 'GSD Type' },
  { key: 'controlPanelNumber', label: 'Control Panel Number' },
  { key: 'plcNumber', label: 'PLC Number' },
  { key: 'plcSlotNumber', label: 'PLC Slot Number' },
  { key: 'fieldPanelNumber', label: 'Field Panel Number' },
  { key: 'dpdpCoupler', label: 'DP/DP Coupler' },
  { key: 'dppaCoupler', label: 'DP/PA Coupler' },
  { key: 'afdHubNumber', label: 'AFD / Hub Number' },
  { key: 'rackNo', label: 'Rack No' },
  { key: 'slotNo', label: 'Slot No' },
  { key: 'channelNo', label: 'Channel No' },
  { key: 'dpNodeAddress', label: 'DP Node Address' },
  { key: 'paNodeAddress', label: 'PA Node Address' },
  { key: 'revision', label: 'Revision' },
  { key: 'revisionChangesOutstandingComments', label: 'Revision Changes / Outstanding Comments' },
  { key: 'zone', label: 'Zone' },
  { key: 'bank', label: 'Bank' },
  { key: 'service', label: 'Service' },
  { key: 'variable', label: 'Variable' },
  { key: 'train', label: 'Train' },
  { key: 'workAreaPack', label: 'Work Area Pack' },
  { key: 'systemCode', label: 'System Code' },
  { key: 'subsystemCode', label: 'Subsystem Code' },
  { key: 'actions', label: 'Actions' },
];

export const nonInstrumentListTableColumns = [
  { key: 'processNo', label: "Process Number" },
  { key: "subProcess", label: "Sub Process" },
  { key: "streamName", label: "Stream Name" },
  { key: "equipmentCode", label: "Equipment Code" },
  { key: "sequenceNumber", label: "Sequence Number" },
  { key: "equipmentIdentifier", label: "Equipment Identifier" },
  { key: "tagName", label: "Tag" },
  { key: "deviceType", label: "Device Type" },
  { key: 'isInstrument', label: 'Is Instrument' },
  { key: 'connectionParentTag', label: 'Connection Parent Tag' },
  { key: 'instrumentParentTag', label: 'Instrument Parent Tag' },
  { key: "serviceDescription", label: "Service Description" },
  { key: "description", label: "Description" },
  { key: "natureOfSignal", label: "Nature Of Signal" },
  { key: "dpNodeAddress", label: "DP Node Address" },
  { key: "noOfSlotsChannels", label: "No Slots/Channels" },
  { key: "slotNumber", label: "Rack Slot Number" },
  { key: "plcNumber", label: "PLC Number" },
  { key: "plcSlotNumber", label: "PLC Slot Number" },
  { key: "location", label: "Location" },
  { key: "manufacturer", label: "Manufacturer" },
  { key: "modelNumber", label: "Model Number" },
  { key: "modelDescription", label: "Model Description" },
  { key: "architectureDrawing", label: "Architecture Drawing" },
  { key: "architectureDrawingSheet", label: "Architecture Drawing Sheet" },
  { key: "revision", label: "Revision" },
  { key: "revisionChanges", label: "Revision Changes" },
  { key: 'actions', label: 'Actions' },
];
//#endregion

//#region For ImportResponse
export const importFileResponseCommonColumns: string[] = ["Status", "Message"];

export const importBankFileColumns: string[] = ["Bank"];

export const importWorkAreaPackFileColumns: string[] = ["Number", "Description"];

export const importTrainFileColumns: string[] = ["Train"];

export const importZoneFileColumns: string[] = ["Zone", "Description", "Area"];

export const importSystemColumns: string[] = ["Number", "Description", "Work Area Pack"];

export const importSubSystemColumns: string[] = ["System", "Number", "Description"];

export const importTagField1Columns: string[] = ["Process Name", "Description"];

export const importTagField2Columns: string[] = ["Sub-Process Name", "Description"];

export const importTagField3Columns: string[] = ["Stream Name", "Description"];

export const importReferenceDocumentColumns: string[] = ["Document Number", "Reference Document Type", "URL", "Description", "Version", "Revision", "Date (MM/dd/yyyy)", "Sheet"];

//export const importTagColumns: string[] = ["Field1Id", "Field1String", "Field2Id", "Field2String", "Field3Id", "Field3String", "Field4Id", "Field4String", "Field5Id", "Field5String", "Field6Id", "Field6String", "TagName"];

export const importJunctionBoxColumns: string[] = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];

export const importPanelColumns: string[] = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];

export const importSkidColumns: string[] = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];

export const importStandColumns: string[] = ["Tag", "Description", "Type", "Area", "Reference Document Type", "Document Number"];

export const importReferenceDocumentType: string[] = ["Type"];

export const importEquipmentCode: string[] = ["Code", "Descriptor"];

export const importFailState: string[] = ["Fail State Name"];

export const importTagTypeColumns: string[] = ["Name", "Description"];

export const importTagDescriptorColumns: string[] = ["Name", "Description"];

export const importManufacturerColumns: string[] = ["Name", "Description", "Comment"];

export const importDeviceModelColumns: string[] = ["Model", "Description", "Manufacturer"];

export const importDeviceTypeColumns = ["Type", "Description"];

export const importNatureOfSignalTypeColumns = ["Name"];

export const importInstrumentColumns: string[] =
 ["PnPId", "Process Number", "Sub Process", "Stream",
  "Equipment Code", "Sequence Number", "Equipment Identifier",
  "Tag", "On Equipment", "On Skid", "Description", "FluidCode",
  "PipeLines.Tag", "Size", "DWG Title", "REV", "VERSION", "To",
  "From", "Tracking Number"];

//#endregion
