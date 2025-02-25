import { DropdownInfoDtoModel } from "@m/common";

export interface UIChangeLogRequestDtoModel {
    projectId: string;
    type: string;
    tag: string | null;
    plcNo: string | null;
    username: string | null;
    startDate: string | null;
    endDate: string | null;
    pageNumber: number | null;
    pageSize: number | null;
}

export interface UIChangeLogTypeDropdownInfoDtoModel {
    types: string[] | null;
    tagList: DropdownInfoDtoModel[] | null;
    plcList: DropdownInfoDtoModel[] | null;
    userList: DropdownInfoDtoModel[] | null;
}