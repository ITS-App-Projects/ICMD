export interface LoginResponseModel extends ResponseModel {
  userDto: LoggedInUserDtoModel;
}
export interface LoggedInUserDtoModel {
  validity: Date;
  profilePic: string;
}
export interface ResponseModel extends BaseResponseModel {
  token: string | null;
  errorType: number;
  action: string;
}
export interface BaseResponseModel {
  message: string;
  isSucceeded: boolean;
  isWarning: boolean;
  data: LoginResponseDataModel;
}
export interface BaseDataResponseModel<TData> {
  message: string;
  isSucceeded: boolean;
  statusCode: number;
  data: TData;
}
export interface LoginResponseDataModel {
  menuPermission: string;
  tag: string;
  tokenExpirationTime: number
}
