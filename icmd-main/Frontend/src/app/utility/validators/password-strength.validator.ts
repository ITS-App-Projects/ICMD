import { AbstractControl, ValidationErrors } from "@angular/forms";

export const PasswordStrengthValidator = (control: AbstractControl): ValidationErrors | null => {
    const value: string = control.value || '';

    if (!value) {
        return null;
    }

    const upperCaseCharacters = /[A-ZА-Я]+/g;
    if (upperCaseCharacters.test(value) === false) {
        return { passwordStrength: 'uppercase' };
    }

    const lowerCaseCharacters = /[a-zа-я]+/g;
    if (lowerCaseCharacters.test(value) === false) {
        return { passwordStrength: 'lowercase' };
    }

    const numberCharacters = /[0-9]+/g;
    if (numberCharacters.test(value) === false) {
        return { passwordStrength: 'number' };
    }

    const specialCharacters = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
    if (specialCharacters.test(value) === false) {
        return { passwordStrength: 'special' };
    }
    return null;
};
