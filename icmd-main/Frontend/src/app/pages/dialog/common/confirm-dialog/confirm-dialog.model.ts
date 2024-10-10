export interface ConfirmDialogInputDataModel {
  header: string;
  confirmationText: string;
  okText: string;
  cancelText: string;
  icon: string;
  showCancel: boolean;
}

export interface ConfirmDialogOutputDataModel {
  confirm: boolean;
}
