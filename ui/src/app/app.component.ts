import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
    ApiValidationIssueDto,
    BookCarRequestDto,
    BookingConfirmationResponseDto,
    SearchCarResponseDto,
    SearchCarsRequestDto
} from './core/models/car-rental.models';
import { CarRentalApiService } from './core/services/car-rental-api.service';
import { BookingLookupComponent } from './features/booking-lookup/booking-lookup.component';
import { BookingConfirmationComponent } from './features/booking/booking-confirmation.component';
import { BookingFormComponent } from './features/booking/booking-form.component';
import { SearchFormComponent } from './features/search/search-form.component';
import { SearchResultsComponent } from './features/search/search-results.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    SearchFormComponent,
    SearchResultsComponent,
    BookingFormComponent,
    BookingConfirmationComponent,
    BookingLookupComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  hasSearched = false;
  isSearching = false;
  offers: SearchCarResponseDto[] = [];
  selectedOffer: SearchCarResponseDto | null = null;
  lastSearchCriteria: SearchCarsRequestDto | null = null;

  searchErrorMessage = '';
  searchValidationErrors: ApiValidationIssueDto[] = [];

  isBooking = false;
  bookingErrorMessage = '';
  bookingValidationErrors: ApiValidationIssueDto[] = [];
  confirmation: BookingConfirmationResponseDto | null = null;

  constructor(private readonly apiService: CarRentalApiService) {}

  onSearchRequested(criteria: SearchCarsRequestDto): void {
    this.hasSearched = true;
    this.isSearching = true;

    this.searchErrorMessage = '';
    this.searchValidationErrors = [];

    this.offers = [];
    this.selectedOffer = null;
    this.confirmation = null;

    this.lastSearchCriteria = criteria;

    this.apiService.searchCars(criteria).subscribe({
      next: (offers) => {
        this.offers = [...offers].sort((left, right) => left.totalPrice - right.totalPrice);
        this.isSearching = false;
      },
      error: (error: unknown) => {
        const parsed = this.apiService.toApiClientError(error);
        this.searchErrorMessage = parsed.message;
        this.searchValidationErrors = parsed.validationErrors;
        this.isSearching = false;
      }
    });
  }

  onOfferSelected(offer: SearchCarResponseDto): void {
    this.selectedOffer = offer;
    this.confirmation = null;
    this.bookingErrorMessage = '';
    this.bookingValidationErrors = [];
  }

  onBookingRequested(request: BookCarRequestDto): void {
    this.isBooking = true;
    this.bookingErrorMessage = '';
    this.bookingValidationErrors = [];

    this.apiService.bookCar(request).subscribe({
      next: (result) => {
        this.confirmation = result;
        this.isBooking = false;
      },
      error: (error: unknown) => {
        const parsed = this.apiService.toApiClientError(error);
        this.bookingErrorMessage = parsed.message;
        this.bookingValidationErrors = parsed.validationErrors;
        this.isBooking = false;
      }
    });
  }
}
