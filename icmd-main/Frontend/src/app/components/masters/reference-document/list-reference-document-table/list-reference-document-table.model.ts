export interface ReferenceDocumentInfoDtoModel {
    id: string;
    documentNumber: string | null;
    referenceDocumentTypeId: string | null;
    referenceDocumentType: string | null;
    url: string;
    description: string;
    version: string;
    revision: string;
    date: string | null;
    sheet: string;
    projectId: string;
}