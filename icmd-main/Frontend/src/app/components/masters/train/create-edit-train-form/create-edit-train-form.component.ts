import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { CreateOrEditTrainDtoModel } from "./create-edit-train-form.model";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { getGroup } from "@u/forms";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";

@Component({
    standalone: true,
    selector: "app-create-edit-train-form",
    templateUrl: "./create-edit-train-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule
    ],
    providers: [],
})
export class CreateOrEditTrainFormComponent extends FormBaseComponent<CreateOrEditTrainDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditTrainDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    train: { vldtr: [Validators.required] },
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}