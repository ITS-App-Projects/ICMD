export interface CreateOrEditProcessDtoModel {
    id: string;
    projectId: string | null;
    processName: string;
    description: string | null;
}