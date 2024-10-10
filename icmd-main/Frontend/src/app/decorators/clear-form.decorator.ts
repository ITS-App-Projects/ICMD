import { FormGroup } from "@angular/forms";

export const ClearForm =
  <TDefaultValuesModel>(defaultValues: TDefaultValuesModel) =>
  (
    target: any,
    propertyName: string,
    descriptor: TypedPropertyDescriptor<() => void>
  ) => {
    const method = descriptor.value;

    // eslint-disable-next-line space-before-function-paren
    descriptor.value = function () {
      const form: FormGroup = arguments[0];
      form.reset();
      const controls = form.controls;
      for (const key in controls) {
        if (controls.hasOwnProperty(key)) {
          const control = controls[key];
          control.setErrors(null);
        }
      }

      return method.apply(this, arguments);
    };
  };
