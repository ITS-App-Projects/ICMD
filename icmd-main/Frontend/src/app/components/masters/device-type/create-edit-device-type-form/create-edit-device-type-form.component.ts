import { ChangeDetectorRef, Component, Input, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { FormBaseComponent } from "@c/shared/forms";
import { FormGroupG, getGroup } from "@u/forms";
import { FormArray, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatIconModule } from "@angular/material/icon";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { ToastrService } from "ngx-toastr";
import { CommonModule } from "@angular/common";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { CreateOrEditAttributeFormComponent } from "@c/masters/attributes/create-edit-attribute-form";
import { CreateOrEditDeviceTypeDtoModel } from "./create-edit-device-type-form.model";
import { AttributesDtoModel } from "@c/masters/device-model/create-edit-device-model-form/create-edit-device-model-form.model";

@Component({
    standalone: true,
    selector: "app-create-edit-device-type-form",
    templateUrl: "./create-edit-device-type-form.component.html",
    imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatAutocompleteModule,
        ReactiveFormsModule,
        CommonModule,
        MatButtonModule, MatIconModule, MatSelectModule, MatCheckboxModule, CreateOrEditAttributeFormComponent],
    providers: [],
})
export class CreateOrEditDeviceTypeFormComponent extends FormBaseComponent<CreateOrEditDeviceTypeDtoModel> {
    @ViewChild(CreateOrEditAttributeFormComponent) attributeForm: CreateOrEditAttributeFormComponent;
    constructor(private _cdr: ChangeDetectorRef, private _toastr: ToastrService) {
        super(
            getGroup<CreateOrEditDeviceTypeDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    type: { vldtr: [Validators.required] },
                    description: {},
                    attributes: new FormArray<FormGroupG<AttributesDtoModel>>([])
                }
            )
        );
    }

    @Input() public set items(val: CreateOrEditDeviceTypeDtoModel) {
        if (val) {
            super.value = {
                id: val?.id,
                type: val?.type,
                description: val?.description,
                attributes: []
            }
            val.attributes?.map((res) => {
                this.attributeForm.addAttribute(res);
            })
            this._cdr.detectChanges();
        }
    }
}