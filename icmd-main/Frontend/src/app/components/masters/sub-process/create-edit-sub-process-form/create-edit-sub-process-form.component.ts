import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { getGroup } from "@u/forms";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { CreateOrEditSubProcessDtoModel } from "./create-edit-sub-process-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-sub-process-form",
    templateUrl: "./create-edit-sub-process-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule
    ],
    providers: [],
})
export class CreateOrEditSubProcessFormComponent extends FormBaseComponent<CreateOrEditSubProcessDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditSubProcessDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    subProcessName: { vldtr: [Validators.required] },
                    description: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}