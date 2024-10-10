export interface TagFieldInfoDtoModel {
    id: string;
    name: string | null;
    source: string | null;
    separator: string | null;
    projectId: string;
    isEditable: boolean;
}

export interface ProjectTagFieldsInfoDtoModel {
    tagFields: Array<TagFieldInfoDtoModel>
}