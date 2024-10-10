import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { AppConfig } from "src/app/app.config";
import { CreateOrEditZoneDtoModel } from "./create-edit-zone-form.model";
import { ZoneInfoDtoModel } from "../list-zone-table";

@Component({
    standalone: true,
    selector: "app-create-edit-zone-form",
    templateUrl: "./create-edit-zone-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule],
    providers: [],
})
export class CreateOrEditZoneFormComponent extends FormBaseComponent<CreateOrEditZoneDtoModel> {
    constructor(private _cdr: ChangeDetectorRef) {
        super(
            getGroup<CreateOrEditZoneDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    zone: { vldtr: [Validators.required] },
                    description: {},
                    area: {},
                    projectId: { vldtr: [Validators.required] },
                }
            )
        );
    }

    public set value(val: CreateOrEditZoneDtoModel) {
        super.value = val;
        this._cdr.detectChanges();
    }

    public get value(): CreateOrEditZoneDtoModel {
        if (this.form.invalid) {
            this.showErrors();
            return;
        }
        const formValue = super.value;
        super.value = {
            ...super.value,
            area: this.isEmptyOrNull(formValue.area)
                ? null
                : formValue.area
        };
        return super.value;
    }

    private isEmptyOrNull(value: number | string | null): boolean {
        return value == null || value?.toString() === "" ? true : false;
    }


}