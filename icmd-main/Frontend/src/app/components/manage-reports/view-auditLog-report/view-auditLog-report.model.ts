import { MatTableDataSource } from "@angular/material/table";

export interface ChangeLogListDtoModel {
    key: string | null;
    items: ChangeLogInfoDtoModel[] | null;
    expanded: boolean;
}


export interface ChangeLogInfoDtoModel {
    context: string | null;
    entityName: string | null;
    contextId: string;
    status: string | null;
    originalValues: string | null;
    newValues: string | null;
    createdDate: string;
    createdBy: string | null;
}