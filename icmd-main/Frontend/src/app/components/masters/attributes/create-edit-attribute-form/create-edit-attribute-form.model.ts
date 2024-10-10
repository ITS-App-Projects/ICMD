import { AttributesDtoModel } from "@c/masters/device-model/create-edit-device-model-form/create-edit-device-model-form.model";

export interface CreateOrEditAttributesDtoModel {
    attributes: AttributesDtoModel[] | null;
}