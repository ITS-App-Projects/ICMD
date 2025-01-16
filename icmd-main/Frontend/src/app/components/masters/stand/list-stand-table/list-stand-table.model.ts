export interface StandListDtoModel {
    id: string;
    tag: string | null;
    process: string | null;
    subProcess: string | null;
    stream: string | null;
    equipmentCode: string | null;
    sequenceNumber: string | null;
    equipmentIdentifier: string | null;
    type: string | null;
    description: string | null;
    referenceDocumentType: string | null;
    documentNumber: string | null;
    revision: string | null;
    version: string | null;
    sheet: string | null;
    isVDPDocumentNumber: boolean;
    projectId: string | null;
    number: string | null;
    isActive: boolean;
    area: string | null;
}