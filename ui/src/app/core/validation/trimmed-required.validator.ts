import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const trimmedRequiredValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value;

  if (typeof value !== 'string') {
    return null;
  }

  return value.trim().length > 0 ? null : { trimmedRequired: true };
};
