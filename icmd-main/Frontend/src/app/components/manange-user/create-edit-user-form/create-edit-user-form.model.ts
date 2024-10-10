export interface CreateOrEditUserModel {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  phoneNumber: string | null;
  password: string;
  confirmPassword: string;
  roleName: string;
}

export interface ViewUserDetails extends CreateOrEditUserModel {
  fullName: string;
}

export interface UpdateUserModel {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string | null;
  roleName: string;
}
