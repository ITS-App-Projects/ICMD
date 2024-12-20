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



import { CreateOrEditCableSubFormComponent } from "@c/masters/cable-sub-type/create-edit-cableSub-form/create-edit-cableSub-form.component";
import { CableSubService } from "src/app/service/cable-sub-type";

@Component({
    standalone: true,
    selector: "app-cable-sub-dialog",
    templateUrl: "./cableSub-add-edit-dialog.component.html",
    providers: [
        CableSubService,
        TagService,
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditCableSubFormComponent,
        MatDialogModule
    ],
})
export class CableSubAddEditDialogComponent {
    @ViewChild(CreateOrEditCableSubFormComponent) cableSubForm: CreateOrEditCableSubFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
        CableSubAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _cableSubService: CableSubService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.cableSubForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._cableSubService.getCableSubInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.cableSubForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveCableSubInfo(): void {
        console.log(this.cableSubForm.value);
        const cableSubInfo = this.cableSubForm.value;
        if (cableSubInfo === null || cableSubInfo == undefined) {
            return;
        }

        
        this.isLoading = !this.isLoading;
        this._cableSubService.createEditCableSub(cableSubInfo).subscribe(
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