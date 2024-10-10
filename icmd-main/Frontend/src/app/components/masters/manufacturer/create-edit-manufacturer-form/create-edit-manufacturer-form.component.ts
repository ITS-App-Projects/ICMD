import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditManufacturerDtoModel } from "./create-edit-manufacturer-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-manufacturer-form",
    templateUrl: "./create-edit-manufacturer-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditManufacturerFormComponent extends FormBaseComponent<CreateOrEditManufacturerDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditManufacturerDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    name: { vldtr: [Validators.required] },
                    description: {},
                    comment: {}
                }
            )
        );
    }
}