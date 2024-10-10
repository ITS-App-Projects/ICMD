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
import { CommonDialogOutputDataModel, CommonSystemDialogInputDataModel } from "@m/common";
import { DocumentTypeService } from "src/app/service/documentType";
import { CreateOrEditDocumentTypeFormComponent } from "@c/masters/documentType/create-edit-document-type-form";

@Component({
    standalone: true,
    selector: "app-document-type-dialog",
    templateUrl: "./document-type-add-edit-dialog.component.html",
    providers: [
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditDocumentTypeFormComponent,
        MatDialogModule
    ],
})
export class DocumentTypeAddEditDialogComponent {
    @ViewChild(CreateOrEditDocumentTypeFormComponent) documentTypeForm: CreateOrEditDocumentTypeFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            DocumentTypeAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._documentTypeService.getDocumentTypeInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.documentTypeForm.value =
                    {
                        id: res?.id,
                        type: res?.type
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveTypeInfo(): void {
        const typeInfo = this.documentTypeForm.value;
        if (typeInfo === null || typeInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._documentTypeService.createEditDocumentType(typeInfo).subscribe(
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