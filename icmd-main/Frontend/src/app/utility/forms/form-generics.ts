import { AbstractControl, AbstractControlOptions, AsyncValidatorFn, FormControl, FormGroup, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';

interface ExtendedState<TFieldValue> {
  value: TFieldValue;
  disabled?: boolean;
}

type FormControlStateG<TFieldValue> = ExtendedState<TFieldValue> | TFieldValue;

type ValidatorG = ValidatorFn | ValidatorFn[] | AbstractControlOptions | null;
type AsyncValidatorG = AsyncValidatorFn | AsyncValidatorFn[] | null;

export class FormControlG<TFieldValue> extends FormControl {
  value: TFieldValue;
  constructor(state: FormControlStateG<TFieldValue>, validatorOrOpts?: ValidatorG, asyncValidator?: AsyncValidatorG) {
    super(state, validatorOrOpts, asyncValidator);
  }
}
type Controls<E> = {
  [K in keyof E]: FormControlG<E[K]>;
};

export class FormGroupG<TFormValue> extends FormGroup {
  value: TFormValue;
  readonly valueChanges: Observable<TFormValue>;

  controls: Controls<TFormValue>;
  constructor(controls: Controls<TFormValue>, validatorOrOpts?: ValidatorG, asyncValidator?: AsyncValidatorG) {
    super(controls, validatorOrOpts, asyncValidator);
  }
}

interface SimpleControlInfo<TValue> {
  v?: TValue;
  vldtr?: ValidatorG;
  avldtr?: AsyncValidatorG;
}

export type ControlInfo<E> = {
  [K in keyof E]: SimpleControlInfo<E[K]> | AbstractControl;
};

export const getGroup = <TFormvalue>(
  info: ControlInfo<TFormvalue>,
  validatorOrOpts?: ValidatorG,
  asyncValidator?: AsyncValidatorG
): FormGroupG<TFormvalue> => {
  const controls: Controls<TFormvalue> = {} as Controls<TFormvalue>;

  Object.keys(info).forEach(
    (key) =>
      (controls[key] =
        info[key] instanceof AbstractControl
          ? info[key]
          : new FormControlG(info[key].v === false ? false : info[key].v === 0 ? 0 : info[key].v || '', info[key].vldtr, info[key].avldtr))
  );

  return new FormGroupG<TFormvalue>(controls, validatorOrOpts, asyncValidator);
};
