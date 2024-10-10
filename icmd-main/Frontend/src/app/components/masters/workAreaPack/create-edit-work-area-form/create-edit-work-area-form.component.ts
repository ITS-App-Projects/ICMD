import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { DropdownInfoDtoModel } from "@m/common";
import { CreateOrEditWorkAreaPackDtoModel } from "./create-edit-work-area-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-work-area-pack-form",
    templateUrl: "./create-edit-work-area-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditWorkAreaPackFormComponent extends FormBaseComponent<CreateOrEditWorkAreaPackDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditWorkAreaPackDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    number: { vldtr: [Validators.required] },
                    description: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}