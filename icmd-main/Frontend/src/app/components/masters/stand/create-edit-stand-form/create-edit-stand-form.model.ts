export interface CreateOrEditStandDtoModel {
    id: string;
    tagId: string | null;
    type: string;
    description: string;
    referenceDocumentTypeId: string | null;
    referenceDocumentId: string | null;
    area: string | null;
}