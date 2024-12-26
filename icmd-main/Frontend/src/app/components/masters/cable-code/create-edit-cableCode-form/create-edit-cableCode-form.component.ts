import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { Subject } from "rxjs";

import { CreateOrEditCableCodeDtoModel } from "./create-edit-cableCode-form.model";
import { AppConfig } from "src/app/app.config";
import { CableTypeListDtoModel } from "@c/masters/cable-type/list-cable-type-table";
import { CableSubListDtoModel } from "@c/masters/cable-sub-type/list-cable-sub-table";

@Component({
    standalone: true,
    selector: "app-create-edit-cableCode-form",
    templateUrl: "./create-edit-cableCode-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditCableCodeFormComponent extends FormBaseComponent<CreateOrEditCableCodeDtoModel> {

    typeInfo: CableTypeListDtoModel[] = [];
    subTypeInfo: CableSubListDtoModel[] = [];
    private _destroy$ = new Subject<void>();

    constructor() {
        super(
            getGroup<CreateOrEditCableCodeDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    projectId: { vldtr: [Validators.required] },

                    cableCode: {},
                    fr: {},
                    type: {},
                    subType: {},
                    size: {},
                    core: {},
                    coreMaterial: {},

                    screen: {},
                    rating: {},
                    innerSheath: {},
                    outerSheath: {},
                    other: {},
                    sheathColour: {},
                    internalCoreColour: {},
                    cableDescription: {},
                    overallDiameter: {},
                    weight: {},
                    comment: {},
                }
            )
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
