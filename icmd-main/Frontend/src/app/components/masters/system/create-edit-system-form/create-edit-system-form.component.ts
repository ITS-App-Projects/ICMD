import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { CreateOrEditSystemDtoModel } from "./create-edit-system-form.model";
import { WorkAreaPackInfoDtoModel } from "@c/masters/workAreaPack/list-work-area-table";

@Component({
    standalone: true,
    selector: "app-create-edit-system-form",
    templateUrl: "./create-edit-system-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditSystemFormComponent extends FormBaseComponent<CreateOrEditSystemDtoModel> {
    public workAreaPacksData: WorkAreaPackInfoDtoModel[] = [];
    constructor() {
        super(
            getGroup<CreateOrEditSystemDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    number: { vldtr: [Validators.required] },
                    description: {},
                    workAreaPackId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}