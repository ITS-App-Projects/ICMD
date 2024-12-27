import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { Subject } from "rxjs";

import { CreateOrEditDesignPackageDtoModel } from "./create-edit-designPackage-form.model";
import { AppConfig } from "src/app/app.config";


@Component({
    standalone: true,
    selector: "app-create-edit-designPackage-form",
    templateUrl: "./create-edit-designPackage-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditDesignPackageFormComponent extends FormBaseComponent<CreateOrEditDesignPackageDtoModel> {

    private _destroy$ = new Subject<void>();

    constructor() {
        super(
            getGroup<CreateOrEditDesignPackageDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    projectId: { vldtr: [Validators.required] },

                    designPackage: {},
                    designPackageName: {},
                    designVendor: {},
                    designLead: {},
                    leadName: {},
                    packageStatus: {},
                }
            )
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
