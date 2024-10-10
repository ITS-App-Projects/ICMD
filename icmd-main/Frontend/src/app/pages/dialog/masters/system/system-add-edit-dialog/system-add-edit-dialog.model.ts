import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";

export interface SystemDialogInputDataModel {
    id: string,
    workAreaPacks: WorkAreaPackInfoDtoModel[];
}