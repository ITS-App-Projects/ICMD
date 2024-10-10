import { Component, EventEmitter, Inject, Input, OnDestroy, Output, ViewChild } from "@angular/core";
import { AbstractControl, FormArray, FormGroup, FormGroupDirective } from "@angular/forms";
import { FormGroupG } from "@u/forms";
import { markFormControlsTouchedHelper, objEqualWithEcxeptions } from "@u/helpers";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { FormBaseSettings } from "./form-base.models";
import { SaveForm } from "@dec/save-form.decorator";

const defaultOptions: FormBaseSettings = {
    watchChanges: false,
    ignoreWatchFields: [],
};

@Component({
   template: '',
   providers: [
       {
           provide: 'defaultOptions',
           useValue: defaultOptions,
       },
   ],
})
export abstract class FormBaseComponent<TModel> implements OnDestroy {
    @Output() ok = new EventEmitter<TModel>();
    // eslint-disable-next-line @angular-eslint/no-output-native
    @Output() reset = new EventEmitter<boolean>();
    @Output() clear = new EventEmitter<boolean>();

    @ViewChild(FormGroupDirective) formGroupDirective: FormGroupDirective;

    @Input() public set value(v: TModel) {
        if (v) {
            this.startValue = v;
            this.form.setValue(v, { emitEvent: false });
        }
    }

    public get value(): TModel | null {
        if (this.form.invalid) {
            this.showErrors();
            return null;
        }
        return this.form.getRawValue() as TModel;
    }

    public get isFormError(): boolean {
        return this.form.touched && this.form.invalid;
    }

    public error$: Subject<boolean> = new Subject<boolean>();
    public changed$: Subject<boolean> = new Subject<boolean>();
    public disabled$: Subject<boolean> = new Subject<boolean>();

    public get changed(): boolean {
        return this._changed;
    }

    protected destroy$: Subject<void> = new Subject<void>();
    protected startValue: TModel;
    protected clearValue: TModel;
    protected settings: FormBaseSettings;

    private _changed = false;

    constructor(public form: FormGroupG<TModel>, @Inject('defaultOptions') settings: Partial<FormBaseSettings> = {}) {
        this.settings = { ...defaultOptions, ...settings };
        this.startValue = form.value;
        this.clearValue = form.value;

        form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((_) => {
            this.error$.next(this.isFormError);
        });

        form.statusChanges.pipe(takeUntil(this.destroy$)).subscribe((_) => {
            this.error$.next(this.isFormError);
        });

        if (this.settings.watchChanges) {
            this.form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value: TModel) => {
                this.setChanged(!objEqualWithEcxeptions(value, this.startValue, this.settings.ignoreWatchFields));
            });
        }
    }

    @SaveForm()
    onSubmit(form: FormGroup): void {
        this.ok.emit(this.value);
    }

    field(name: string, subGroup?: string): AbstractControl {
        let form = this.form as FormGroup;
        if (subGroup) {
            form = form.get(subGroup) as FormGroup;
            if (!form) {
                throw new Error(`from group name '${subGroup}' is not found`);
            }
        }

        return this.fieldByGroup(form, name);
    }

    fieldByGroup(group: FormGroup, fieldName: string): AbstractControl {
        const field = group.get(fieldName);
        if (!field) {
            throw new Error(`field name '${fieldName}' is not found`);
        }
        return field;
    }

    array = <TGroup = any>(name: string, subGroup?: string): FormArray<FormGroupG<TGroup>> =>
        this.field(name, subGroup) as FormArray<FormGroupG<TGroup>>;

    isError(fieldName: string, error: string, subGroup?: string): boolean {
        const field = this.field(fieldName, subGroup);
        return this.isErrorByField(field, error);
    }

    isErrorByGroup(group: FormGroup, fieldName: string, error: string): boolean {
        const field = this.fieldByGroup(group, fieldName);
        return this.isErrorByField(field, error);
    }

    isErrorByField(field: AbstractControl, error: string): boolean {
        const result = field.invalid && field.touched && field.errors ? field.errors[error] : false;
        return result;
    }

    showErrors(): void {
        markFormControlsTouchedHelper(this.form);
        this.error$.next(this.isFormError);
    }

    resetForm(): void {
        this.formGroupDirective.resetForm(this.startValue);
        this.error$.next(false);
        if (this.settings.watchChanges) {
            this.setChanged(false);
        }
    }

    pristineForm(): void {
        this.form.markAsPristine();
    }

    clearForm(defaultModel: Partial<TModel> = {}): void {
        this.formGroupDirective.resetForm(defaultModel);
        this.setChanged(false);
    }

    onReset(): void {
        this.resetForm();
        this.reset.emit(true);
    }

    onClear(): void {
        this.clearForm();
        this.clear.emit(true);
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
        this.error$.complete();
    }

    disableForm(internal = true): void {
        if (internal) {
            this.form.disable();
        }
        this.disabled$.next(true);
    }

    enableFrom(internal = true): void {
        if (internal) {
            this.form.enable();
        }
        this.disabled$.next(false);
    }

   protected setChanged(val: boolean): void {
       this._changed = val;
       this.changed$.next(val);
   }
}