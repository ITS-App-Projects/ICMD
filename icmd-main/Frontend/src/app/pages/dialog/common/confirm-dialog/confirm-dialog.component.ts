import { CommonModule } from "@angular/common";
import { Component, Inject, OnInit } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

import {
  ConfirmDialogInputDataModel,
  ConfirmDialogOutputDataModel,
} from "./confirm-dialog.model";
import { MatCommonModule } from "@angular/material/core";

const defaultConfirmData: ConfirmDialogInputDataModel = {
  header: "Confirm",
  confirmationText: "Are You sure?",
  okText: "Ok",
  cancelText: "Cancel",
  icon: "contact_support",
  showCancel: true,
};

@Component({
  standalone: true,
  selector: "app-confirm-dialog",
  templateUrl: "confirm-dialog.component.html",
  styleUrls: ["./styles/confirm-dialog.component.scss"],
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
})
export class ConfirmDialogComponent implements OnInit {
  readonly data: ConfirmDialogInputDataModel;

  constructor(
    private _dialogRef: MatDialogRef<
      ConfirmDialogComponent,
      ConfirmDialogOutputDataModel
    >,
    @Inject(MAT_DIALOG_DATA) data: Partial<ConfirmDialogInputDataModel>
  ) {
    this.data = { ...defaultConfirmData, ...data };
  }

  ngOnInit(): void { }

  ok(): void {
    this._dialogRef.close({ confirm: true });
  }

  cancel(): void {
    this._dialogRef.close({ confirm: false });
  }
}
