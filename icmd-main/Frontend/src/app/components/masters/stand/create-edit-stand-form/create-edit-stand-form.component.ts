import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { DropdownInfoDtoModel } from "@m/common";
import { ReferenceDocumentService } from "src/app/service/reference-document";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { CreateOrEditStandDtoModel } from "./create-edit-stand-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-stand-form",
    templateUrl: "./create-edit-stand-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [ReferenceDocumentService],
})
export class CreateOrEditStandFormComponent extends FormBaseComponent<CreateOrEditStandDtoModel> {
    projectId: string = null;
    tagInfo: DropdownInfoDtoModel[] = [];
    typeInfo: DropdownInfoDtoModel[] = [];
    documentsInfo: DropdownInfoDtoModel[] = [];
    private _destroy$ = new Subject<void>();

    constructor(private _referenceDocumentService: ReferenceDocumentService) {
        super(
            getGroup<CreateOrEditStandDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    tagId: { vldtr: [Validators.required] },
                    type: {},
                    description: {},
                    referenceDocumentTypeId: {},
                    referenceDocumentId: {},
                    area: {}
                }
            )
        );
    }

    public get value(): CreateOrEditStandDtoModel {
        if (this.form.invalid) {
            this.showErrors();
            return;
        }

        const formValue = super.value;
        super.value = {
            ...super.value,
            referenceDocumentId: this.isEmptyOrNull(
                formValue.referenceDocumentId
            )
                ? null
                : formValue.referenceDocumentId,
            referenceDocumentTypeId: this.isEmptyOrNull(
                formValue.referenceDocumentTypeId
            )
                ? null
                : formValue.referenceDocumentTypeId,
        };
        return super.value;
    }

    public set value(val: CreateOrEditStandDtoModel) {
        if (val) {
            super.value = val;
            if (val.referenceDocumentId) {
                this.getDocumentInfo(val.referenceDocumentTypeId, val.referenceDocumentId);
            }
        }
    }

    protected getDocumentInfo(documentTypeId: string, documentId: string = null): void {
        if (this.projectId && documentTypeId) {
            this._referenceDocumentService.getAllDocumentInfo(this.projectId, documentTypeId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.documentsInfo = res;
                    if (documentId != null) {
                        this.field('referenceDocumentId').setValue(documentId);
                    }
                })
        }
        else {
            this.field('referenceDocumentId').setValue(null);
            this.documentsInfo = [];
        }
        this.field('referenceDocumentId').updateValueAndValidity();
    }

    private isEmptyOrNull(value: number | string | null): boolean {
        return value == null || value?.toString() === "" ? true : false;
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}