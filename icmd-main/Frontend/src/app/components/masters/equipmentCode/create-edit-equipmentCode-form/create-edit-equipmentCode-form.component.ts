import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditEquipmentCodeDtoModel } from "./create-edit-equipmentCode-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-equipmentCode-form",
    templateUrl: "./create-edit-equipmentCode-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditEquipmentCodeFormComponent extends FormBaseComponent<CreateOrEditEquipmentCodeDtoModel> {
    constructor() {
        super(
            getGroup<CreateOrEditEquipmentCodeDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    code: { vldtr: [Validators.required] },
                    descriptor: {},
                }
            )
        );
    }
}