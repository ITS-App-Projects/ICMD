import { DropdownInfoDtoModel } from "@m/common";

//#region Getting Parent
export interface HierarchyRequestDtoModel {
    projectId: string | null;
    hieararchyType: string | null;
    option: string | null;
    tagName: string | null;
}
export interface HierarchyResponceDtoModel {
    deviceList: HierarchyDeviceInfoDtoModel[];
    tagList: DropdownInfoDtoModel[];
}

//#region Getting Children
export interface ChildrenRequestDtoModel {
    deviceId: string | null;
    projectId: string | null;
    option: string | null;
    hieararchyType: string | null;
}
export interface ChildrenResponseDtoModel {
    deviceList: HierarchyDeviceInfoDtoModel[];
}

//#region Tree Data Model
export interface HierarchyDeviceInfoDtoModel {
    id: string;
    name: string | null;
    instrument: boolean;
    isFolder: boolean;
    isActive: boolean;
    childrenList?: HierarchyDeviceInfoDtoModel[] | null;
}
export interface ExampleFlatNode {
    expandable: boolean;
    name: string;
    level: number;
}