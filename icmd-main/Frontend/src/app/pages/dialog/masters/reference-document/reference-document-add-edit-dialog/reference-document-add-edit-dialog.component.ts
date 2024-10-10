import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { HttpErrorResponse } from "@angular/common/http";
import { takeUntil } from "rxjs/operators";
import { CommonDialogOutputDataModel } from "@m/common";
import { ReferenceDocumentService } from "src/app/service/reference-document";
import { CreateOrEditReferenceDocumentFormComponent } from "@c/masters/reference-document/create-edit-reference-document-form";
import { ReferenceDocumentDialogInputDataModel } from "./reference-document-add-edit-dialog.model";

@Component({
    standalone: true,
    selector: "app-reference-document-dialog",
    templateUrl: "./reference-document-add-edit-dialog.component.html",
    providers: [
        ReferenceDocumentService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditReferenceDocumentFormComponent,
        MatDialogModule
    ],
})
export class ReferenceDocumentAddEditDialogComponent {
    @ViewChild(CreateOrEditReferenceDocumentFormComponent) referenceDocumentForm: CreateOrEditReferenceDocumentFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            ReferenceDocumentAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: ReferenceDocumentDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _referenceDocumentService: ReferenceDocumentService
    ) { }

    ngAfterViewInit(): void {
        this.referenceDocumentForm.documentType = this._inputData.documentType;
        if (this._inputData.projectId) {
            this.referenceDocumentForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._referenceDocumentService.getReferenceDocumentInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.referenceDocumentForm.value =
                    {
                        id: res?.id,
                        documentNumber: res?.documentNumber,
                        description: res?.description,
                        url: res?.url,
                        revision: res?.revision,
                        version: res?.version,
                        sheet: res?.sheet,
                        date: res?.date,
                        referenceDocumentTypeId: res?.referenceDocumentTypeId,
                        projectId: res?.projectId
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveDocumentInfo(): void {
        const documentInfo = this.referenceDocumentForm.value;
        if (documentInfo === null || documentInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._referenceDocumentService.createEditReferenceDocument(documentInfo).subscribe(
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