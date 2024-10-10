export interface CreateOrEditReferenceDocumentDtoModel {
    id: string;
    projectId: string | null;
    referenceDocumentTypeId: string | null;
    documentNumber: string;
    url: string;
    description: string;
    version: string;
    revision: string;
    date: string | null;
    sheet: string;
}