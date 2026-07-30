import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn } from '@angular/forms';
import { SearchCarsRequestDto } from '../../core/models/car-rental.models';

const dateRangeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const from = control.get('from')?.value as string;
  const to = control.get('to')?.value as string;

  if (!from || !to) {
    return null;
  }

  return from < to ? null : { dateOrder: true };
};

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search-form.component.html',
  styleUrl: './search-form.component.css'
})
export class SearchFormComponent {
  @Output() readonly searchRequested = new EventEmitter<SearchCarsRequestDto>();
  private readonly formBuilder = inject(FormBuilder);

  readonly categories = ['', 'Economy', 'Compact', 'SUV', 'Minivan'];

  readonly form = this.formBuilder.group(
    {
      pickup: [''],
      from: [''],
      to: [''],
      category: ['']
    },
    { validators: [dateRangeValidator] }
  );

  submit(): void {
    this.form.markAllAsTouched();

    const pickup = (this.form.value.pickup ?? '').trim();
    const from = this.form.value.from ?? '';
    const to = this.form.value.to ?? '';
    const category = (this.form.value.category ?? '').trim();

    if (!pickup || !from || !to || this.form.hasError('dateOrder')) {
      return;
    }

    this.searchRequested.emit({
      pickup,
      from,
      to,
      category: category || undefined
    });
  }
}
