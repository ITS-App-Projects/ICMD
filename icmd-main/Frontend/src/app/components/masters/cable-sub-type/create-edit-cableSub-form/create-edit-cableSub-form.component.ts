import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";

import { Subject } from "rxjs";
import { CreateOrEditCableSubDtoModel } from "./create-edit-cableSub-form.model";
import { AppConfig } from "src/app/app.config";

@Component({
    standalone: true,
    selector: "app-create-edit-cableSub-form",
    templateUrl: "./create-edit-cableSub-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditCableSubFormComponent extends FormBaseComponent<CreateOrEditCableSubDtoModel> {
    private _destroy$ = new Subject<void>();

    constructor() {
        super(
            getGroup<CreateOrEditCableSubDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    projectId: { vldtr: [Validators.required] },
                    cableSubType: { vldtr: [Validators.required] },
                    description: {},
                }
            )
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
