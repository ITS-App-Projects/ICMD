import { FormGroup } from '@angular/forms';

export const markFormControlsTouchedHelper = (form: FormGroup, subGroupName?: string): void => {
  let f = form;

  if (subGroupName) {
    f = form.get(subGroupName) as FormGroup;
    if (!f) {
      throw new Error(`sub group name ${subGroupName} cannot be find.`);
    }
  }

  const controls = f.controls;
  for (const key in controls) {
    if (controls.hasOwnProperty(key)) {
      const control = controls[key];
      control.markAsTouched();
    }
  }
};
