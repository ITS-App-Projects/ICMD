export interface CreateOrEditZoneDtoModel {
    id: string;
    projectId: string | null;
    zone: string;
    description: string;
    area: number | null;
}