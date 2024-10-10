import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { AbstractControl, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { ProjectTagFieldInfoDtoModel } from "@m/common";
import { CreateOrEditTagDtoModel, GenerateTagDtoModel } from "./create-edit-tag-form.model";
import { FormGroupG, getGroup } from "@u/forms";

@Component({
    standalone: true,
    selector: "app-create-edit-tag-form",
    templateUrl: "./create-edit-tag-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditTagFormComponent extends FormBaseComponent<CreateOrEditTagDtoModel> {
    @Output() public generateTagData = new EventEmitter<GenerateTagDtoModel>();
    tagFieldData: ProjectTagFieldInfoDtoModel[] = [];
    protected isGenerate: boolean = false;
    constructor() {
        super(
            getGroup<CreateOrEditTagDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    field1Id: {},
                    field1String: {},
                    field2Id: {},
                    field2String: {},
                    field3Id: {},
                    field3String: {},
                    field4Id: {},
                    field4String: {},
                    field5Id: {},
                    field5String: {},
                    field6Id: {},
                    field6String: {},
                    projectId: { vldtr: [Validators.required] },
                    tagName: { disabled: true, vldtr: [Validators.required] },
                }
            )
        );
    }

    @Input() public set items(value: ProjectTagFieldInfoDtoModel[]) {
        this.tagFieldData = value;
        if (this.tagFieldData) {
            this.tagFieldData.map((res, index) => {
                if (res.isUsed) {
                    if (res.fieldData == null) {
                        this.field('field' + (index + 1) + 'Id').disable({ onlySelf: true });
                    }
                    else {
                        this.field('field' + (index + 1) + 'String').disable({ onlySelf: true });
                    }
                }
                else {
                    this.field('field' + (index + 1) + 'Id').disable({ onlySelf: true });
                    this.field('field' + (index + 1) + 'String').disable({ onlySelf: true });
                }
            })
        }
        this.form.valueChanges.subscribe((res) => {
            this.enableDisableGenerateBtn(this.form);
        })
    }

    public get value(): CreateOrEditTagDtoModel {
        if (this.form.invalid) {
            this.showErrors();
            return;
        }
        const field1Id = this.form.get('field1Id');
        const field1String = this.form.get('field1String');
        const field2Id = this.form.get('field2Id');
        const field2String = this.form.get('field2String');
        const field3Id = this.form.get('field3Id');
        const field3String = this.form.get('field3String');
        const field4Id = this.form.get('field4Id');
        const field4String = this.form.get('field4String');
        const field5Id = this.form.get('field5Id');
        const field5String = this.form.get('field5String');
        const field6Id = this.form.get('field6Id');
        const field6String = this.form.get('field6String');
        const formValue = super.value;
        super.value = {
            ...super.value,
            field1Id: this.isEmptyOrNull(
                formValue.field1Id
            )
                ? null
                : (!field1Id.disabled ? formValue.field1Id : null),
            field2Id: this.isEmptyOrNull(
                formValue.field2Id
            )
                ? null
                : (!field2Id.disabled ? formValue.field2Id : null),
            field3Id: this.isEmptyOrNull(
                formValue.field3Id
            )
                ? null
                : (!field3Id.disabled ? formValue.field3Id : null),
            field4Id: this.isEmptyOrNull(
                formValue.field4Id
            )
                ? null
                : (!field4Id.disabled ? formValue.field4Id : null),
            field5Id: this.isEmptyOrNull(
                formValue.field5Id
            )
                ? null
                : (!field5Id.disabled ? formValue.field5Id : null),
            field6Id: this.isEmptyOrNull(
                formValue.field6Id
            )
                ? null
                : (!field6Id.disabled ? formValue.field6Id : null),
            field1String: this.isEmptyOrNull(
                formValue.field1String
            )
                ? null
                : (!field1String.disabled ? formValue.field1String : null),
            field2String: this.isEmptyOrNull(
                formValue.field2String
            )
                ? null
                : (!field2String.disabled ? formValue.field2String : null),
            field3String: this.isEmptyOrNull(
                formValue.field3String
            )
                ? null
                : (!field3String.disabled ? formValue.field3String : null),
            field4String: this.isEmptyOrNull(
                formValue.field4String
            )
                ? null
                : (!field4String.disabled ? formValue.field4String : null),
            field5String: this.isEmptyOrNull(
                formValue.field5String
            )
                ? null
                : (!field5String.disabled ? formValue.field5String : null),
            field6String: this.isEmptyOrNull(
                formValue.field6String
            )
                ? null
                : (!field6String.disabled ? formValue.field6String : null),

        };
        return super.value;
    }

    public set value(val: CreateOrEditTagDtoModel) {
        if (val) {
            super.value = val;
            this.enableDisableGenerateBtn(this.form);
        }
    }

    protected getControlName(index: number, isId: boolean): string {
        return `field${index + 1}` + (isId ? 'Id' : 'String');
    }

    protected generateTag(): void {
        const field1Id = this.form.get('field1Id');
        const field1String = this.form.get('field1String');
        const field2Id = this.form.get('field2Id');
        const field2String = this.form.get('field2String');
        const field3Id = this.form.get('field3Id');
        const field3String = this.form.get('field3String');
        const field4Id = this.form.get('field4Id');
        const field4String = this.form.get('field4String');
        const field5Id = this.form.get('field5Id');
        const field5String = this.form.get('field5String');
        const field6Id = this.form.get('field6Id');
        const field6String = this.form.get('field6String');

        const fields = [field1Id, field2Id, field3Id, field4Id, field5Id, field6Id];
        const fieldString = [field1String, field2String, field3String, field4String, field5String, field6String];
        const fieldValue = [null, null, null, null, null, null];

        fields.forEach((field, index) => {
            if (field.disabled) {
                fieldValue[index] = fieldString[index].value;
            } else {
                fieldValue[index] = field.value;
            }
        });

        let generateTagInfo: GenerateTagDtoModel = {
            field1Id: fieldValue[0],
            field2Id: fieldValue[1],
            field3Id: fieldValue[2],
            field4Id: fieldValue[3],
            field5Id: fieldValue[4],
            field6Id: fieldValue[5],
            projectId: ""
        }

        this.generateTagData.emit(generateTagInfo);

    }

    private isEmptyOrNull(value: number | string | null): boolean {
        return value == null || value?.toString() === "" ? true : false;
    }

    private enableDisableGenerateBtn(form: FormGroupG<CreateOrEditTagDtoModel>): void {
        this.isGenerate = false;

        let generate: boolean = false;
        const field1Id = this.form.get('field1Id');
        const field1String = this.form.get('field1String');
        const field2Id = this.form.get('field2Id');
        const field2String = this.form.get('field2String');
        const field3Id = this.form.get('field3Id');
        const field3String = this.form.get('field3String');
        const field4Id = this.form.get('field4Id');
        const field4String = this.form.get('field4String');
        const field5Id = this.form.get('field5Id');
        const field5String = this.form.get('field5String');
        const field6Id = this.form.get('field6Id');
        const field6String = this.form.get('field6String');
        const fields = [field1Id, field1String, field2Id, field2String, field3Id, field3String, field4Id, field4String, field5Id, field5String,
            field6Id, field6String];

        for (const field of fields) {
            if (!field.disabled && this.isFieldEmpty(field)) {
                generate = true;
            }
        }

        this.isGenerate = !generate;
    }

    private isFieldEmpty(field: AbstractControl): boolean {
        return (field.value == null || field.value === '');
    }

}