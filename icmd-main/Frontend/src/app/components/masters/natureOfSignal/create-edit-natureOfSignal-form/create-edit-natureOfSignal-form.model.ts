import { AttributesDtoModel } from "@c/masters/device-model/create-edit-device-model-form/create-edit-device-model-form.model";

export interface CreateOrEditNatureOfSignalDtoModel {
    id: string;
    natureOfSignalName: string;
    attributes: AttributesDtoModel[] | null;
}