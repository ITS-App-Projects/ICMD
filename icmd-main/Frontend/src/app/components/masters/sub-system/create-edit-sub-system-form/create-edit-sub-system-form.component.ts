import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditSubSystemDtoModel } from "./create-edit-sub-system-form.model";
import { SystemInfoDtoModel } from "@c/masters/system/list-system-table";

@Component({
    standalone: true,
    selector: "app-create-edit-sub-system-form",
    templateUrl: "./create-edit-sub-system-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditSubSystemFormComponent extends FormBaseComponent<CreateOrEditSubSystemDtoModel> {
    public systemData: SystemInfoDtoModel[] = [];
    constructor() {
        super(
            getGroup<CreateOrEditSubSystemDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    number: { vldtr: [Validators.required] },
                    description: {},
                    systemId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}