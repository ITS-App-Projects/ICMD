import { DropdownInfoDtoModel } from "@m/common";

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