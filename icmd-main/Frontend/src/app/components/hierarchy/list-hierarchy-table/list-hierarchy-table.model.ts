import { signal } from '@angular/core';
import { DropdownInfoDtoModel } from '@m/common';

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
export class ExampleFlatNode {
    id: string;
    name: string;
    expandable: boolean;
    level: number;

    constructor(id: string,  name: string, expandable: boolean, level: number)
    {
        this.id = id;
        this.name = name;
        this.expandable = expandable;
        this.level = level;
    }
}

/** Flat node with expandable and level information */
export class DynamicFlatNode {
    constructor(
      public item: string,
      public level = 1,
      public expandable = false,
      public isLoading = signal(false)
    ) {}
  }