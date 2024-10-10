export interface CreateOrEditDeviceModelDtoModel {
    id: string;
    model: string;
    description: string;
    manufacturerId: string | null;
    attributes: AttributesDtoModel[] | null;
}

export interface AttributesDtoModel {
    id: string;
    name: string;
    description: string;
    valueType: string | null;
    private: boolean;
    inherit: boolean;
    required: boolean;
    value: string;
}