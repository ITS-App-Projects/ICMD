import { FormGroup, ValidatorFn } from "@angular/forms";

export const mustMatch =
  (controlName: string, matchingControlName: string, isHideOnOtherErrors: boolean): ValidatorFn =>
    (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (isHideOnOtherErrors && matchingControl.errors && !matchingControl.errors.mustMatch) {
        return null;
      }

      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      }
    };
