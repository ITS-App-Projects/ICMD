import { AttributesDtoModel } from "@c/masters/device-model/create-edit-device-model-form/create-edit-device-model-form.model";

export interface CreateOrEditDeviceTypeDtoModel {
    id: string;
    type: string;
    description: string;
    attributes: AttributesDtoModel[] | null;
}