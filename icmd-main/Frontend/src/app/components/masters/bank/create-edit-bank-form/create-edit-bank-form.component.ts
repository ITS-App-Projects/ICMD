import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { CreateOrEditBankDtoModel } from "./create-edit-bank-form.model";
import { getGroup } from "@u/forms";
import { DropdownInfoDtoModel } from "@m/common";
import { AppConfig } from "src/app/app.config";

@Component({
    standalone: true,
    selector: "app-create-edit-bank-form",
    templateUrl: "./create-edit-bank-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditBankFormComponent extends FormBaseComponent<CreateOrEditBankDtoModel> {
    constructor(private appConfig: AppConfig) {
        super(
            getGroup<CreateOrEditBankDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    bank: { vldtr: [Validators.required] },
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }
}