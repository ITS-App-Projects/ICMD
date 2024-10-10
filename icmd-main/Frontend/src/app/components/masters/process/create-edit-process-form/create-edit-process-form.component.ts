import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { getGroup } from "@u/forms";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { CreateOrEditProcessDtoModel } from "./create-edit-process-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-process-form",
    templateUrl: "./create-edit-process-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule
    ],
    providers: [],
})
export class CreateOrEditProcessFormComponent extends FormBaseComponent<CreateOrEditProcessDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditProcessDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    processName: { vldtr: [Validators.required] },
                    description: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}