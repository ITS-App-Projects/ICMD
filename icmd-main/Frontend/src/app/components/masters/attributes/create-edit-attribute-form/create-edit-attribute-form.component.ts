import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component } from "@angular/core";
import { FormArray, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { AttributesDtoModel } from "@c/masters/device-model/create-edit-device-model-form/create-edit-device-model-form.model";
import { FormBaseComponent } from "@c/shared/forms";
import { AttributeTypes } from "@e/common";
import { FormGroupG, getGroup } from "@u/forms";
import { CreateOrEditAttributesDtoModel } from "./create-edit-attribute-form.model";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-create-edit-attribute-form",
    templateUrl: "./create-edit-attribute-form.component.html",
    imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        ReactiveFormsModule,
        CommonModule,
        MatButtonModule, MatIconModule, MatSelectModule, MatCheckboxModule, NgScrollbarModule],
    providers: [],
})
export class CreateOrEditAttributeFormComponent extends FormBaseComponent<CreateOrEditAttributesDtoModel> {
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    protected attributesTypes = AttributeTypes;
    protected attributes: string[] = [];
    constructor(private _cdr: ChangeDetectorRef) {
        super(
            getGroup<CreateOrEditAttributesDtoModel>(
                {
                    attributes: new FormArray<FormGroupG<AttributesDtoModel>>([])
                }
            )
        );
        const keys = Object.keys(this.attributesTypes);
        this.attributes = keys.slice(keys.length / 2);
    }

    public addAttribute(info: AttributesDtoModel = null): void {
        const attributeList = this.array("attributes");
        const valueType = this.attributes?.[0] ?? null;
        attributeList.push(
            getGroup<AttributesDtoModel>({
                id: { v: info?.id ?? this.emptyGuid },
                name: { v: info?.name ?? null, vldtr: [Validators.required] },
                description: { v: info?.description ?? null },
                valueType: { v: info?.valueType ?? valueType },
                private: { v: info?.private ?? false },
                inherit: { v: info?.inherit ?? false },
                required: { v: info?.required ?? false },
                value: { v: info?.value ?? null },
            })
        );
    }

    protected getGroup(index: number): FormGroupG<AttributesDtoModel> {
        return this.array('attributes').controls[index] as FormGroupG<AttributesDtoModel>;
    }

    protected deleteAttribute(index: number): void {
        this.array("attributes").removeAt(index);
    }

    protected changeValueType(index: number): void {
        this.getGroup(index).get('value').setValue(null);
        this._cdr.detectChanges();
    }

    protected numberOnly(event): boolean {
        const charCode = event.which ? event.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
}