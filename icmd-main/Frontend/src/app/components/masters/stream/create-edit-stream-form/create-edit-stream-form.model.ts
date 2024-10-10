export interface CreateOrEditStreamDtoModel {
    id: string;
    projectId: string | null;
    streamName: string;
    description: string | null;
}