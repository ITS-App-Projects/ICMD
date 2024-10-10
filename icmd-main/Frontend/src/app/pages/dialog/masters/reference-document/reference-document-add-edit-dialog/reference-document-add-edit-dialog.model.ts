import { DropdownInfoDtoModel } from "@m/common";

export interface ReferenceDocumentDialogInputDataModel {
    id: string,
    projectId: string;
    documentType: DropdownInfoDtoModel[];
}