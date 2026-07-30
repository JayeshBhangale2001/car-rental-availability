import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import {
    ApiValidationIssueDto,
    BookCarRequestDto,
    SearchCarResponseDto,
    SearchCarsRequestDto
} from '../../core/models/car-rental.models';

const DOMESTIC_LOCATIONS = ['mumbai', 'delhi'];

@Component({
  selector: 'app-booking-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-form.component.html',
  styleUrl: './booking-form.component.css'
})
export class BookingFormComponent {
  private readonly formBuilder = inject(FormBuilder);

  @Input({ required: true }) selectedOffer!: SearchCarResponseDto;
  @Input({ required: true }) searchCriteria!: SearchCarsRequestDto;
  @Input() isSubmitting = false;
  @Input() errorMessage = '';
  @Input() validationErrors: ApiValidationIssueDto[] = [];

  @Output() readonly bookingRequested = new EventEmitter<BookCarRequestDto>();

  localDocumentError = '';

  readonly form = this.formBuilder.group({
    driverName: [''],
    documentType: [''],
    documentNumber: ['']
  });

  get expectedDocumentType(): string {
    const pickup = this.searchCriteria.pickup.trim().toLowerCase();
    return DOMESTIC_LOCATIONS.includes(pickup) ? 'NationalId' : 'Passport';
  }

  submit(): void {
    this.form.markAllAsTouched();
    this.localDocumentError = '';

    const driverName = (this.form.value.driverName ?? '').trim();
    const documentType = (this.form.value.documentType ?? '').trim();
    const documentNumber = (this.form.value.documentNumber ?? '').trim();

    if (!driverName || !documentType || !documentNumber) {
      return;
    }

    if (documentType !== this.expectedDocumentType) {
      this.localDocumentError = `For ${this.searchCriteria.pickup}, document type must be ${this.expectedDocumentType}.`;
      return;
    }

    this.bookingRequested.emit({
      provider: this.selectedOffer.provider,
      offerId: this.selectedOffer.offerId,
      driverName,
      documentType,
      documentNumber,
      pickup: this.searchCriteria.pickup,
      from: this.searchCriteria.from,
      to: this.searchCriteria.to
    });
  }
}
