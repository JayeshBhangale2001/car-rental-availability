import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { BookingDetailsResponseDto } from '../../core/models/car-rental.models';
import { CarRentalApiService } from '../../core/services/car-rental-api.service';

@Component({
  selector: 'app-booking-lookup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-lookup.component.html',
  styleUrl: './booking-lookup.component.css'
})
export class BookingLookupComponent implements OnChanges {
  private readonly formBuilder = inject(FormBuilder);

  @Input() suggestedReference = '';

  isLoading = false;
  errorMessage = '';
  booking: BookingDetailsResponseDto | null = null;

  readonly form = this.formBuilder.group({
    reference: ['']
  });

  constructor(
    private readonly apiService: CarRentalApiService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['suggestedReference'] && this.suggestedReference) {
      this.form.patchValue({ reference: this.suggestedReference });
    }
  }

  lookup(): void {
    this.form.markAllAsTouched();
    const reference = (this.form.value.reference ?? '').trim();
    if (!reference) {
      return;
    }

    this.booking = null;
    this.errorMessage = '';
    this.isLoading = true;

    this.apiService.getBookingByReference(reference).subscribe({
      next: (result) => {
        this.booking = result;
        this.isLoading = false;
      },
      error: (error: unknown) => {
        const parsed = this.apiService.toApiClientError(error);
        this.errorMessage = parsed.message;
        this.isLoading = false;
      }
    });
  }
}
