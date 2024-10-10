export interface ChangePasswordModel {
    userId: string;
    oldPassword: string;
    newPassword: string;
    confirmPassword: string;
}