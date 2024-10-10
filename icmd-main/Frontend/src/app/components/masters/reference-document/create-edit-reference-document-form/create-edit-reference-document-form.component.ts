import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditReferenceDocumentDtoModel } from "./create-edit-reference-document-form.model";
import { DropdownInfoDtoModel } from "@m/common";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatNativeDateModule } from "@angular/material/core";

@Component({
    standalone: true,
    selector: "app-create-edit-reference-document-form",
    templateUrl: "./create-edit-reference-document-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule,
        MatDatepickerModule, MatNativeDateModule],
    providers: [],
})
export class CreateOrEditReferenceDocumentFormComponent extends FormBaseComponent<CreateOrEditReferenceDocumentDtoModel> {
    documentType: DropdownInfoDtoModel[] = [];
    constructor() {
        super(
            getGroup<CreateOrEditReferenceDocumentDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    documentNumber: { vldtr: [Validators.required] },
                    referenceDocumentTypeId: { vldtr: [Validators.required] },
                    url: {},
                    sheet: {},
                    revision: {},
                    version: {},
                    date: {},
                    description: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }

    public get value(): CreateOrEditReferenceDocumentDtoModel {
        if (this.form.invalid) {
            this.showErrors();
            return;
        }

        const formValue = super.value;
        super.value = {
            ...super.value,
            date: this.isEmptyOrNull(
                formValue.date
            )
                ? null
                : formValue.date,
            referenceDocumentTypeId: this.isEmptyOrNull(
                formValue.referenceDocumentTypeId
            )
                ? null
                : formValue.referenceDocumentTypeId,
        };
        return super.value;
    }

    public set value(val: CreateOrEditReferenceDocumentDtoModel) {
        if (val) {
            super.value = val;
        }
    }

    private isEmptyOrNull(value: number | string | null): boolean {
        return value == null || value?.toString() === "" ? true : false;
    }
}