export interface ColumnTemplateDialogInputDataModel {
    userDefinedTemplates: TemplateListModel[],
    currentSelectedColumns: string
}

export interface ColumnTemplateDialogOutputDataModel {
    success: boolean;
    id: string;
}

export interface TemplateListModel {
    id: string,
    property: string;
    value: string | null;
    isDefault: boolean;
}

export interface CreateMetaDataModel {
    templateName: string;
    value: string;
}