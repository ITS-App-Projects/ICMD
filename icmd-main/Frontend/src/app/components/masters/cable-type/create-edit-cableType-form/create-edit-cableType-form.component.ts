import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";

import { Subject } from "rxjs";
import { CreateOrEditCableTypeDtoModel } from "./create-edit-cableType-form.model"; 
import { AppConfig } from "src/app/app.config";

@Component({
    standalone: true,
    selector: "app-create-edit-cableType-form",
    templateUrl: "./create-edit-cableType-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditCableTypeFormComponent extends FormBaseComponent<CreateOrEditCableTypeDtoModel> {
    private _destroy$ = new Subject<void>();

    constructor() {
        super(
            getGroup<CreateOrEditCableTypeDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    projectId: { vldtr: [Validators.required] },
                    cableType: { vldtr: [Validators.required] },
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
