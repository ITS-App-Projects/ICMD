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
import { CreateOrEditTagTypeFormComponent } from "@c/masters/tagType/create-edit-tagType-form";
import { TagDescriptorService } from "src/app/service/tagDescriptor";

@Component({
    standalone: true,
    selector: "app-tagDescriptor-dialog",
    templateUrl: "./tagDescriptor-add-edit-dialog.component.html",
    providers: [
        TagDescriptorService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditTagTypeFormComponent,
        MatDialogModule
    ],
})
export class TagDescriptorAddEditDialogComponent {
    @ViewChild(CreateOrEditTagTypeFormComponent) tagDescriptorForm: CreateOrEditTagTypeFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            TagDescriptorAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _tagDescriptorService: TagDescriptorService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.id != null) {
            this._tagDescriptorService.getTagDescriptorInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.tagDescriptorForm.value =
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

    protected saveTagDescriptorInfo(): void {
        const tagDescriptorForm = this.tagDescriptorForm.value;
        if (tagDescriptorForm === null || tagDescriptorForm == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._tagDescriptorService.createEditTagDescriptor(tagDescriptorForm).subscribe(
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