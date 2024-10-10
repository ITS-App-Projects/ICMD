import { Component } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { MenuService } from "src/app/service/menu";
import { CreateOrEditMenuModel } from "./create-or-edit-menu-form.model";
import { getGroup } from "@u/forms";
import { Validators } from "@angular/forms";
import { MatCheckboxModule } from "@angular/material/checkbox";

@Component({
    standalone: true,
    selector: "app-create-edit-menu-form",
    templateUrl: "./create-or-edit-menu-form.component.html",

    imports: [FormDefaultsModule, MatIconModule, MatCheckboxModule],
    providers: [
        MenuService,

    ],
})
export class CreateEditMenuFormComponent extends FormBaseComponent<CreateOrEditMenuModel> {
    constructor() {
        super(
            getGroup<CreateOrEditMenuModel>({
                id: { v: "00000000-0000-0000-0000-000000000000" },
                menuName: { vldtr: [Validators.required] },
                menuDescription: { vldtr: [Validators.required] },
                controllerName: { vldtr: [Validators.required] },
                icon: {},
                url: {},
                sortOrder: { v: 0 },
                parentMenuId: {},
                isPermission: { v: true },
            })
        );
    }

    ngOnInit(): void { }
}