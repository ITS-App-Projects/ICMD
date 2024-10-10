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
import { TagTypeService } from "src/app/service/tagType";
import { CreateOrEditTagTypeFormComponent } from "@c/masters/tagType/create-edit-tagType-form";

@Component({
    standalone: true,
    selector: "app-tagType-dialog",
    templateUrl: "./tagType-add-edit-dialog.component.html",
    providers: [
        TagTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditTagTypeFormComponent,
        MatDialogModule
    ],
})
export class TagTypeAddEditDialogComponent {
    @ViewChild(CreateOrEditTagTypeFormComponent) tagTypeForm: CreateOrEditTagTypeFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            TagTypeAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _tagTypeService: TagTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._tagTypeService.getTagTypeInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.tagTypeForm.value =
                    {
                        id: res?.id,
                        name: res?.name,
                        description: res?.description
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveTagTypeInfo(): void {
        const tagTypeInfo = this.tagTypeForm.value;
        if (tagTypeInfo === null || tagTypeInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._tagTypeService.createEditTagType(tagTypeInfo).subscribe(
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