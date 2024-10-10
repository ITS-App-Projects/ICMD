export interface CreateOrEditSubProcessDtoModel {
    id: string;
    projectId: string | null;
    subProcessName: string;
    description: string | null;
}