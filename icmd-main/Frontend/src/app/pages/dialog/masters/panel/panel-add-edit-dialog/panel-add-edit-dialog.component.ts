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
import { PanelService } from "src/app/service/panel";

@Component({
    standalone: true,
    selector: "app-panel-dialog",
    templateUrl: "./panel-add-edit-dialog.component.html",
    providers: [
        PanelService,
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
export class PanelAddEditDialogComponent {
    @ViewChild(CreateOrEditJunctionBoxFormComponent) panelForm: CreateOrEditJunctionBoxFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            PanelAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _panelService: PanelService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId != null) {
            this.panelForm.projectId = this._inputData.projectId;
            combineLatest([
                this._tagService.getProjectWiseTagInfo(this._inputData.projectId, "Panel", this._inputData.id),
                this._documentTypeService.getAllDocumentTypeInfo()
            ]).pipe(takeUntil(this._destroy$)).subscribe(([tagInfo, typeInfo]) => {
                this.panelForm.tagInfo = tagInfo;
                this.panelForm.typeInfo = typeInfo;
            });
        }

        if (this._inputData.id != null) {
            this._panelService.getPanelnfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.panelForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected savePanelInfo(): void {
        const panelInfo = this.panelForm.value;
        if (panelInfo === null || panelInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._panelService.createEditPanel(panelInfo).subscribe(
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