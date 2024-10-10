import { ChangeDetectorRef, Component, Input, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { FormGroupG, getGroup } from "@u/forms";
import { FormArray, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatIconModule } from "@angular/material/icon";
import { AttributeTypes, AuthorizationType } from "@e/common";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelect, MatSelectModule } from "@angular/material/select";
import { ToastrService } from "ngx-toastr";
import { CommonModule } from "@angular/common";
import { BehaviorSubject, Observable } from "rxjs";
import { AttributesDtoModel, CreateOrEditDeviceModelDtoModel } from "./create-edit-device-model-form.model";
import { DropdownInfoDtoModel } from "@m/common";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { CreateOrEditAttributeFormComponent } from "@c/masters/attributes/create-edit-attribute-form";

@Component({
    standalone: true,
    selector: "app-create-edit-device-model-form",
    templateUrl: "./create-edit-device-model-form.component.html",
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
export class CreateOrEditDeviceModelFormComponent extends FormBaseComponent<CreateOrEditDeviceModelDtoModel> {
    @ViewChild(CreateOrEditAttributeFormComponent) attributeForm: CreateOrEditAttributeFormComponent;
    manufacturersData: DropdownInfoDtoModel[] = [];
    constructor(private _cdr: ChangeDetectorRef, private _toastr: ToastrService) {
        super(
            getGroup<CreateOrEditDeviceModelDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    model: { vldtr: [Validators.required] },
                    description: {},
                    manufacturerId: { vldtr: [Validators.required] },
                    attributes: new FormArray<FormGroupG<AttributesDtoModel>>([])
                }
            )
        );
    }

    @Input() public set items(val: CreateOrEditDeviceModelDtoModel) {
        if (val) {
            super.value = {
                id: val?.id,
                model: val?.model,
                description: val?.description,
                manufacturerId: val?.manufacturerId,
                attributes: []
            }
            val.attributes?.map((res) => {
                this.attributeForm.addAttribute(res);
            })
            this._cdr.detectChanges();
        }
    }
}