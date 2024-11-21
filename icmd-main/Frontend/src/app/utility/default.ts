
export const pageSizeOptions = [10, 25, 50, 100];

export const defaultPageSize = pageSizeOptions[0];

export const dashboardPageSizeOptions = [3, 5, 10];

export const dashboardDefaultPageSize = dashboardPageSizeOptions[0];

//Guard Permission parameter
export const permissionMenuName: string = "permissionMenuName";
export const requiredOperation: string = "requiredOperation";

//Column Selector Memory Cache Key
export const listColumnMemoryCacheKey = {
    instrumentColumn: "InstrumentList",
    nonInstrumentColumn: "NonInstrumentList",
    manageUser: "ManageUsers",
    manageProject: "ManageProjects",
    workAreaPack: "WorkAreaPack",
    zones: "Zones",
    systems: "Systems",
    subSystem: "SubSystem",
    tagField1: "TagField1",
    tagField2: "TagField2",
    tagField3: "TagField3",
    referenceDocument: "ReferenceDocument",
    tags: "Tags",
    junctionBox: "JunctionBox",
    panel: "Panel",
    skid: "Skid",
    stand: "Stand",    
    equipmentCode: "EquipmentCode",
    tagTypes: "TagTypes",
    tagDescriptors: "TagDescriptors",
    manufacturer: "Manufacturer",
    deviceModel: "DeviceModel",
    deviceType: "DeviceType"
}