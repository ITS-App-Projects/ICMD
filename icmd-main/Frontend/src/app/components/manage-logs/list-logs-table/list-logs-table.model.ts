export interface ChangeLogItemDtoModel {
    type: string | null;
    tag: string | null;
    date: string;
    status: string | null;
    userName: string | null;
    properties: PropertyChangeLogDtoModel[];
    referenceDocuments: ReferenceDocumentChangeLogDtoModel[];
    attributes: PropertyChangeLogDtoModel[];
    statuses: PropertyChangeLogDtoModel[];
    bulkDeleteRecords: BulkDeleteLogDtoModel[];
    importRecords: BulkDeleteLogDtoModel[];
}

export interface PropertyChangeLogDtoModel {
    name: string | null;
    oldValue: string | null;
    newValue: string | null;
}

export interface ReferenceDocumentChangeLogDtoModel {
    type: string | null;
    documentNo: string | null;
    revision: string | null;
    version: string | null;
    sheet: string | null;
    status: string | null;
}

export interface ChangeLogResponceDtoModel {
    key: string;
    items: ChangeLogItemDtoModel[];
}

export interface BulkDeleteLogDtoModel {
    name: string | null;
    status: boolean | null;
    message: string | null;
}

export interface ImportLogDtoModel {
    name: string | null;
    status: string | null;
    message: string | null;
    operation: string | null;
    items: ImportLogChangesDtoModel[];
}

export interface ImportLogChangesDtoModel {
    itemColumnName: string | null;
    previousValue: string | null;
    newValue: string | null;
}