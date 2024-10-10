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
import { StandService } from "src/app/service/stand";
import { CreateOrEditStandFormComponent } from "@c/masters/stand/create-edit-stand-form";

@Component({
    standalone: true,
    selector: "app-stand-dialog",
    templateUrl: "./stand-add-edit-dialog.component.html",
    providers: [
        StandService,
        TagService,
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditStandFormComponent,
        MatDialogModule
    ],
})
export class StandAddEditDialogComponent {
    @ViewChild(CreateOrEditStandFormComponent) standForm: CreateOrEditStandFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            StandAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _standService: StandService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId != null) {
            this.standForm.projectId = this._inputData.projectId;
            combineLatest([
                this._tagService.getProjectWiseTagInfo(this._inputData.projectId, "Stand", this._inputData.id),
                this._documentTypeService.getAllDocumentTypeInfo()
            ]).pipe(takeUntil(this._destroy$)).subscribe(([tagInfo, typeInfo]) => {
                this.standForm.tagInfo = tagInfo;
                this.standForm.typeInfo = typeInfo;
            });
        }

        if (this._inputData.id != null) {
            this._standService.getStandInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.standForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveStandInfo(): void {
        const standInfo = this.standForm.value;
        if (standInfo === null || standInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._standService.createEditStand(standInfo).subscribe(
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