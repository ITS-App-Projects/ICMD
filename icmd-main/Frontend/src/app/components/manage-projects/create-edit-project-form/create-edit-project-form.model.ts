export interface CreateOrEditProjectDtoModel {
    id: string;
    name: string;
    description: string | null;
    userAuthorizations: UserAuthorizationDtoModel[];
}

export interface UserAuthorizationDtoModel {
    id: string;
    userId: string;
    authorization: string | null;
}