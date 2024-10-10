export interface CreateOrEditTagDtoModel {
    id: string;
    field1Id: string | null;
    field2Id: string | null;
    field3Id: string | null;
    field4Id: string | null;
    field5Id: string | null;
    field6Id: string | null;
    field1String: string | null;
    field2String: string | null;
    field3String: string | null;
    field4String: string | null;
    field5String: string | null;
    field6String: string | null;
    tagName: string;
    projectId: string;
}

export interface TagInfoDtoModel {
    id: string;
    tagName: string | null;
    sequenceNumber: string | null;
    equipmentIdentifier: string | null;
    processId: string;
    subProcessId: string;
    streamId: string;
    equipmentCodeId: string;
}

export interface GenerateTagDtoModel {
    field1Id: string | null;
    field2Id: string | null;
    field3Id: string | null;
    field4Id: string | null;
    field5Id: string | null;
    field6Id: string | null;
    projectId: string;
}