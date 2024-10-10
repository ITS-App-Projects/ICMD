import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { CommonDialogOutputDataModel } from "@m/common";
import { Subject } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { HttpErrorResponse } from "@angular/common/http";
import { TagService } from "src/app/service/tag";
import { TagDialogInputDataModel } from "./tag-add-edit-dialog.model";
import { CreateOrEditTagFormComponent, GenerateTagDtoModel } from "@c/masters/tag/create-edit-tag-form";
import { takeUntil } from "rxjs/operators";
import { ProjectService } from "src/app/service/manage-projects";

@Component({
    standalone: true,
    selector: "app-tag-dialog",
    templateUrl: "./tag-add-edit-dialog.component.html",
    providers: [
        TagService,
        ProjectService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditTagFormComponent,
        MatDialogModule
    ],
})
export class TagAddEditDialogComponent {
    @ViewChild(CreateOrEditTagFormComponent) tagForm: CreateOrEditTagFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            TagAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: TagDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _tagService: TagService,
        private _projectService: ProjectService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this._projectService.getProjectTagFieldSourcesDataInfo(this._inputData.projectId).subscribe((res) => {
                this.tagForm.items = res;
                this._cdr.detectChanges();
            })
        }

        this.tagForm.field('projectId').setValue(this._inputData.projectId);
        this.tagForm.field('projectId').updateValueAndValidity();
        if (this._inputData.id != null) {
            this._tagService.getTagInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.tagForm.value = res;
                    this._cdr.detectChanges();
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveTagInfo(): void {
        const tagInfo = this.tagForm.value;
        if (tagInfo === null || tagInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._tagService.createEditTag(tagInfo).subscribe(
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

    protected generateTag(tagInfo: GenerateTagDtoModel): void {
        if (this._inputData.projectId) {
            tagInfo.projectId = this._inputData.projectId;
            this._tagService.generateTag(tagInfo).subscribe(
                (res) => {
                    if (res && res?.data && res?.data?.tag) {
                        this.tagForm.field('tagName').setValue(res?.data?.tag ?? "");
                    }
                    else {
                        this.tagForm.field('tagName').setValue(null);
                    }
                    this.tagForm.field('tagName').updateValueAndValidity();
                }
                ,
                (errorRes: HttpErrorResponse) => {
                    this.isLoading = !this.isLoading;
                    if (errorRes?.error?.message) {
                        this._toastr.error(errorRes?.error?.message);
                    }
                }
            );
        }
    }


    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}