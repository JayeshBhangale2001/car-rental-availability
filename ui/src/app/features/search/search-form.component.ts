import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { PickupLocationResponseDto, SearchCarsRequestDto } from '../../core/models/car-rental.models';
import { trimmedRequiredValidator } from '../../core/validation/trimmed-required.validator';

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
  @Input() isSearching = false;
  @Input()
  set pickupLocations(value: PickupLocationResponseDto[]) {
    this._pickupLocations = value;
    this.syncPickupControlState();
  }

  get pickupLocations(): PickupLocationResponseDto[] {
    return this._pickupLocations;
  }

  @Input()
  set isLoadingPickupLocations(value: boolean) {
    this._isLoadingPickupLocations = value;
    this.syncPickupControlState();
  }

  get isLoadingPickupLocations(): boolean {
    return this._isLoadingPickupLocations;
  }

  @Input() pickupLocationsErrorMessage = '';

  @Output() readonly searchRequested = new EventEmitter<SearchCarsRequestDto>();
  private readonly formBuilder = inject(FormBuilder);
  private _pickupLocations: PickupLocationResponseDto[] = [];
  private _isLoadingPickupLocations = false;

  readonly categories = ['', 'Economy', 'Compact', 'SUV', 'Minivan'];

  readonly form = this.formBuilder.group(
    {
      pickup: this.formBuilder.control({ value: '', disabled: true }, [Validators.required, trimmedRequiredValidator]),
      from: ['', Validators.required],
      to: ['', Validators.required],
      category: ['']
    },
    { validators: [dateRangeValidator] }
  );

  get domesticPickupLocations(): PickupLocationResponseDto[] {
    return this.pickupLocations.filter(location => location.locationType === 'Domestic');
  }

  get internationalPickupLocations(): PickupLocationResponseDto[] {
    return this.pickupLocations.filter(location => location.locationType === 'International');
  }

  private syncPickupControlState(): void {
    const pickupControl = this.form.controls.pickup;
    const shouldDisable = this.isLoadingPickupLocations || this.pickupLocations.length === 0;

    if (shouldDisable) {
      pickupControl.disable({ emitEvent: false });
      pickupControl.setValue('', { emitEvent: false });
      return;
    }

    pickupControl.enable({ emitEvent: false });
  }

  submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const pickup = (this.form.value.pickup ?? '').trim();
    const from = this.form.value.from ?? '';
    const to = this.form.value.to ?? '';
    const category = (this.form.value.category ?? '').trim();
    const pickupLocationType = this.pickupLocations
      .find(location => location.name.toLowerCase() === pickup.toLowerCase())
      ?.locationType;

    this.searchRequested.emit({
      pickup,
      pickupLocationType,
      from,
      to,
      category: category || undefined
    });
  }
}
