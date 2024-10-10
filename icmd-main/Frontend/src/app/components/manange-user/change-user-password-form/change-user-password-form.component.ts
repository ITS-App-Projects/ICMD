import { Component, ChangeDetectorRef } from "@angular/core";
import { Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { FormDefaultsModule, FormBaseComponent } from "@c/shared/forms";
import { getGroup } from "@u/forms";
import { PasswordStrengthValidator, mustMatch } from "@u/validators";
import { ChangeUserPasswordModel } from ".";

@Component({
    standalone: true,
    selector: "app-change-user-password-form",
    templateUrl: "./change-user-password-form.component.html",
    imports: [
        FormDefaultsModule,
        MatIconModule,
        MatButtonModule,
    ],
    providers: [],
})
export class ChangeUserPasswordFormComponent extends FormBaseComponent<ChangeUserPasswordModel> {
    constructor(
        private _cdr: ChangeDetectorRef
    ) {
        super(
            getGroup<ChangeUserPasswordModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    password: {
                        vldtr: [
                            Validators.required,
                            Validators.minLength(8),
                            PasswordStrengthValidator,
                        ],
                    },
                    confirmPassword: { vldtr: [Validators.required] }
                },
                mustMatch("password", "confirmPassword", true)
            )
        );
    }

    ngOnInit(): void { }
}