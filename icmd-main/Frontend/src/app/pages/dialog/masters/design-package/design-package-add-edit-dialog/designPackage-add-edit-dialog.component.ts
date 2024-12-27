import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject, combineLatest } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { HttpErrorResponse } from "@angular/common/http";
import { takeUntil } from "rxjs/operators";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { TagService } from "src/app/service/tag";
import { DocumentTypeService } from "src/app/service/documentType";

import { CreateOrEditDesignPackageFormComponent } from "@c/masters/design-package/create-edit-designPackage-form/create-edit-designPackage-form.component";
import { DesignPackageService } from "src/app/service/design-package";

@Component({
    standalone: true,
    selector: "app-design-package-dialog",
    templateUrl: "./designPackage-add-edit-dialog.component.html",
    providers: [
        DesignPackageService,
        TagService,
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditDesignPackageFormComponent,
        MatDialogModule
    ],
})
export class DesignPackageAddEditDialogComponent {
    @ViewChild(CreateOrEditDesignPackageFormComponent) designPackageForm: CreateOrEditDesignPackageFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
        CreateOrEditDesignPackageFormComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _designPackageService: DesignPackageService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.designPackageForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._designPackageService.getDesignPackageInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.designPackageForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveDesignPackageInfo(): void {
        console.log(this.designPackageForm.value);
        const designPackageInfo = this.designPackageForm.value;
        if (designPackageInfo === null || designPackageInfo == undefined) {
            return;
        }

        
        this.isLoading = !this.isLoading;
        this._designPackageService.createEditDesignPackage(designPackageInfo).subscribe(
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