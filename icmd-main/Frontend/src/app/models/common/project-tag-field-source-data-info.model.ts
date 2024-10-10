export interface ProjectTagFieldInfoDtoModel {
    id: string;
    name: string | null;
    source: string | null;
    isUsed: boolean;
    fieldData: SourceDataInfoDtoModel[] | null;
}

export interface SourceDataInfoDtoModel {
    id: string;
    name: string | null;
}