import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { CreateOrEditBankFormComponent } from "@c/masters/bank/create-edit-bank-form";
import { HttpErrorResponse } from "@angular/common/http";
import { BankService } from "src/app/service/bank";
import { takeUntil } from "rxjs/operators";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";

@Component({
    standalone: true,
    selector: "app-bank-dialog",
    templateUrl: "./bank-add-edit-dialog.component.html",
    providers: [
        BankService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditBankFormComponent,
        MatDialogModule
    ],
})
export class BankAddEditDialogComponent {
    @ViewChild(CreateOrEditBankFormComponent) bankForm: CreateOrEditBankFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            BankAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _bankService: BankService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.bankForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._bankService.getBankInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.bankForm.value =
                    {
                        id: res?.id,
                        bank: res?.bank,
                        projectId: res?.projectId
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveBankInfo(): void {
        const bankInfo = this.bankForm.value;
        if (bankInfo === null || bankInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._bankService.createEditBank(bankInfo).subscribe(
            (res) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._dialogRef.close({ success: true });
                } else {
                    this.isLoading = !this.isLoading;
                    this._toastr.error(res.message);
                }
            },
            (errorRes: HttpErrorResponse) => {
                this.isLoading = !this.isLoading;
                if (errorRes?.error?.message) {
                    this._toastr.error(errorRes?.error?.message);
                }
            }
        );
    }


    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}