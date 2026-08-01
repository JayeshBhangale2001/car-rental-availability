export interface SearchCarsRequestDto {
  pickup: string;
  pickupLocationType?: string;
  from: string;
  to: string;
  category?: string;
}

export interface PickupLocationResponseDto {
  name: string;
  locationType: string;
}

export interface SearchCarResponseDto {
  provider: string;
  offerId: string;
  vehicleName: string;
  category: string;
  perDayRate: number;
  totalPrice: number;
  cancellationPolicy: string;
  insuranceIncluded: boolean;
  currency: string;
}

export interface BookCarRequestDto {
  provider: string;
  offerId: string;
  driverName: string;
  documentType: string;
  documentNumber: string;
  pickup: string;
  from: string;
  to: string;
}

export interface BookingConfirmationResponseDto {
  reference: string;
  provider: string;
  category: string;
  totalPrice: number;
  cancellationPolicy: string;
}

export interface BookingOfferResponseDto {
  provider: string;
  offerId: string;
  vehicleName: string;
  category: string;
  perDayRate: number;
  totalPrice: number;
  cancellationPolicy: string;
  insuranceType: string;
  insuranceIncluded: boolean;
  currency: string;
}

export interface BookingDetailsResponseDto {
  reference: string;
  provider: string;
  driverName: string;
  documentType: string;
  documentNumber: string;
  pickup: string;
  pickupLocationType: string;
  from: string;
  to: string;
  offer: BookingOfferResponseDto;
  bookedAtUtc: string;
}

export interface ApiValidationIssueDto {
  kind: string;
  field: string;
  code: string;
  message: string;
}

export interface ApiValidationErrorResponseDto {
  errors: ApiValidationIssueDto[];
}

export interface BookingNotFoundResponseDto {
  message: string;
}

export interface ApiClientError {
  status: number;
  message: string;
  validationErrors: ApiValidationIssueDto[];
}
