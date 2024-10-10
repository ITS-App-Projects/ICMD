import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";

export interface SubSystemDialogInputDataModel {
    id: string,
    systems: SystemInfoDtoModel[];
}