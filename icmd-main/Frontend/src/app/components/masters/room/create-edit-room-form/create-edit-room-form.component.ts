import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule, Validators } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { Subject } from "rxjs";
import { AppConfig } from "src/app/app.config";
import { CreateOrEditRoomDtoModel } from "./create-edit-room-form.model";


@Component({
    standalone: true,
    selector: "app-create-edit-room-form",
    templateUrl: "./create-edit-room-form.component.html",
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatSelectModule],
    providers: [],
})
export class CreateOrEditRoomFormComponent extends FormBaseComponent<CreateOrEditRoomDtoModel> {

    private _destroy$ = new Subject<void>();

    constructor() {
        super(
            getGroup<CreateOrEditRoomDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    projectId: { vldtr: [Validators.required] },

                    room: {},
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
