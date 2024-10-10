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
import { CreateOrEditJunctionBoxFormComponent } from "@c/masters/junction-box/create-edit-junction-box-form";
import { TagService } from "src/app/service/tag";
import { DocumentTypeService } from "src/app/service/documentType";
import { SkidService } from "src/app/service/skid";

@Component({
    standalone: true,
    selector: "app-skid-dialog",
    templateUrl: "./skid-add-edit-dialog.component.html",
    providers: [
        SkidService,
        TagService,
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditJunctionBoxFormComponent,
        MatDialogModule
    ],
})
export class SkidAddEditDialogComponent {
    @ViewChild(CreateOrEditJunctionBoxFormComponent) skidForm: CreateOrEditJunctionBoxFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            SkidAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _skidService: SkidService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId != null) {
            this.skidForm.projectId = this._inputData.projectId;
            combineLatest([
                this._tagService.getProjectWiseTagInfo(this._inputData.projectId, "Skid", this._inputData.id),
                this._documentTypeService.getAllDocumentTypeInfo()
            ]).pipe(takeUntil(this._destroy$)).subscribe(([tagInfo, typeInfo]) => {
                this.skidForm.tagInfo = tagInfo;
                this.skidForm.typeInfo = typeInfo;
            });
        }

        if (this._inputData.id != null) {
            this._skidService.getSkidInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.skidForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveSkidInfo(): void {
        const skidInfo = this.skidForm.value;
        if (skidInfo === null || skidInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._skidService.createEditSkid(skidInfo).subscribe(
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