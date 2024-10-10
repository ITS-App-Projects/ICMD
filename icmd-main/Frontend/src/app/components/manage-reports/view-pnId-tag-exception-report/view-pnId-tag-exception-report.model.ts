export interface PnIDTagExceptionInfoDtoModel {
    key: string | null;
    items: ViewPnIDTagExceptionDtoModel[];
    expanded: boolean;
}

export interface ViewPnIDTagExceptionDtoModel {
    tagName: string | null;
    equipmentCode: string | null;
    processName: string | null;
    subProcessName: string | null;
    streamName: string | null;
    sequenceNumber: string | null;
    equipmentIdentifier: string | null;
    serviceDescription: string | null;
    skidTag: string | null;
    projectId: string | null;
}