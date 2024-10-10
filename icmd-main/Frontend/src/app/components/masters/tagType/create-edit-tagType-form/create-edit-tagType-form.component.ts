import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditTagTypeDescriptionDtoModel } from "./create-edit-tagType-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-tagType-form",
    templateUrl: "./create-edit-tagType-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditTagTypeFormComponent extends FormBaseComponent<CreateOrEditTagTypeDescriptionDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditTagTypeDescriptionDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    name: { vldtr: [Validators.required] },
                    description: {}
                }
            )
        );
    }
}