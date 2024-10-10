import { RoleDetailsModel } from "@m/common";

export interface UserDialogInputDataModel {
    id: string;
    roles: RoleDetailsModel[]
}

export interface UserDialogOutputDataModel {
    success: boolean;
}
