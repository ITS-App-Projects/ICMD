export interface ChangeLogItemDtoModel {
    type: string | null;
    tag: string | null;
    date: string;
    status: string | null;
    userName: string | null;
    properties: PropertyChangeLogDtoModel[];
    referenceDocuments: ReferenceDocumentChangeLogDtoModel[];
    attributes: PropertyChangeLogDtoModel[];
    statues: PropertyChangeLogDtoModel[];
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
