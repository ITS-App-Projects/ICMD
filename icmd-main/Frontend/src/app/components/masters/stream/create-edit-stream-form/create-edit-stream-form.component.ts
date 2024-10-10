import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { getGroup } from "@u/forms";
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { CreateOrEditStreamDtoModel } from "./create-edit-stream-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-stream-form",
    templateUrl: "./create-edit-stream-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule
    ],
    providers: [],
})
export class CreateOrEditStreamFormComponent extends FormBaseComponent<CreateOrEditStreamDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditStreamDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    streamName: { vldtr: [Validators.required] },
                    description: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}