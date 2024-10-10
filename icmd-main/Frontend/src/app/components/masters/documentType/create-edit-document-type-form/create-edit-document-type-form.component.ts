import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms"
import { getGroup } from "@u/forms";
import { CreateOrEditReferenceDocumentTypeModel } from "./create-edit-document-type-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-document-type-form",
    templateUrl: "./create-edit-document-type-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule
    ],
    providers: [],
})
export class CreateOrEditDocumentTypeFormComponent extends FormBaseComponent<CreateOrEditReferenceDocumentTypeModel> {
    constructor() {
        super(
            getGroup<CreateOrEditReferenceDocumentTypeModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    type: { vldtr: [Validators.required] }
                }
            )
        );
    }
}