import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    ApiClientError,
    ApiValidationErrorResponseDto,
    BookCarRequestDto,
    BookingConfirmationResponseDto,
    BookingDetailsResponseDto,
    BookingNotFoundResponseDto,
    SearchCarResponseDto,
    SearchCarsRequestDto
} from '../models/car-rental.models';

@Injectable({
  providedIn: 'root'
})
export class CarRentalApiService {
  private readonly baseUrl = `${environment.apiBaseUrl}/cars`;

  constructor(private readonly httpClient: HttpClient) {}

  searchCars(request: SearchCarsRequestDto): Observable<SearchCarResponseDto[]> {
    let params = new HttpParams()
      .set('pickup', request.pickup)
      .set('from', request.from)
      .set('to', request.to);

    if (request.category) {
      params = params.set('category', request.category);
    }

    return this.httpClient.get<SearchCarResponseDto[]>(`${this.baseUrl}/search`, { params });
  }

  bookCar(request: BookCarRequestDto): Observable<BookingConfirmationResponseDto> {
    return this.httpClient.post<BookingConfirmationResponseDto>(`${this.baseUrl}/book`, request);
  }

  getBookingByReference(reference: string): Observable<BookingDetailsResponseDto> {
    return this.httpClient.get<BookingDetailsResponseDto>(`${this.baseUrl}/booking/${encodeURIComponent(reference)}`);
  }

  toApiClientError(error: unknown): ApiClientError {
    if (!(error instanceof HttpErrorResponse)) {
      return {
        status: 0,
        message: 'An unexpected error occurred.',
        validationErrors: []
      };
    }

    const validationPayload = error.error as ApiValidationErrorResponseDto | undefined;
    const notFoundPayload = error.error as BookingNotFoundResponseDto | undefined;
    const validationErrors = Array.isArray(validationPayload?.errors) ? validationPayload.errors : [];

    if (validationErrors.length > 0) {
      return {
        status: error.status,
        message: 'Please fix the highlighted validation errors.',
        validationErrors
      };
    }

    if (typeof notFoundPayload?.message === 'string' && notFoundPayload.message.length > 0) {
      return {
        status: error.status,
        message: notFoundPayload.message,
        validationErrors: []
      };
    }

    if (typeof error.error === 'string' && error.error.length > 0) {
      return {
        status: error.status,
        message: error.error,
        validationErrors: []
      };
    }

    return {
      status: error.status,
      message: error.message || 'Request failed.',
      validationErrors: []
    };
  }
}
