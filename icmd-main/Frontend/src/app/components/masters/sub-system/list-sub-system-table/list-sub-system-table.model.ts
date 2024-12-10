export interface SubSystemInfoDtoModel {
    checked?: boolean;
    id: string;
    number: string | null;
    description: string | null;
    system: string | null;
    workAreaPack: string | null;
    workAreaPackId: string | null;
    systemId: string;
    projectId: string | null;
}