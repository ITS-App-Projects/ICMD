import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditFailStateDtoModel } from "./create-edit-failState-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-failState-form",
    templateUrl: "./create-edit-failState-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditFailStateFormComponent extends FormBaseComponent<CreateOrEditFailStateDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditFailStateDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    failStateName: { vldtr: [Validators.required] }
                }
            )
        );
    }
}