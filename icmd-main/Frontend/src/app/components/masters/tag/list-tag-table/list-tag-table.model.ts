export interface TagListDtoModel {
    id: string;
    tag: string | null;
    field1String: string | null;
    field2String: string | null;
    field3String: string | null;
    field4String: string | null;
    field5String: string | null;
    field6String: string | null;
    projectId: string;
}

export interface TagListImportResultModel {
    key: string;
    value: string;
}