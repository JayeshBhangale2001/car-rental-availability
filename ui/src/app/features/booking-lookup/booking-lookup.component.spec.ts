import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { CarRentalApiService } from '../../core/services/car-rental-api.service';
import { BookingLookupComponent } from './booking-lookup.component';

describe('BookingLookupComponent', () => {
  let fixture: ComponentFixture<BookingLookupComponent>;
  let component: BookingLookupComponent;
  let apiService: jasmine.SpyObj<CarRentalApiService>;

  beforeEach(async () => {
    apiService = jasmine.createSpyObj<CarRentalApiService>('CarRentalApiService', [
      'getBookingByReference',
      'toApiClientError'
    ]);

    await TestBed.configureTestingModule({
      imports: [BookingLookupComponent],
      providers: [
        { provide: CarRentalApiService, useValue: apiService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(BookingLookupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('prefills the reference when a suggested reference is provided', () => {
    fixture.componentRef.setInput('suggestedReference', 'BK-2024');
    fixture.detectChanges();

    expect(component.form.value.reference).toBe('BK-2024');
  });

  it('shows validation feedback and does not call the API when the form is empty', () => {
    component.lookup();
    fixture.detectChanges();

    expect(apiService.getBookingByReference).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Booking reference is required.');
  });

  it('loads and renders a booking for a valid reference', () => {
    apiService.getBookingByReference.and.returnValue(of({
      reference: 'BK-2024',
      provider: 'BudgetWheels',
      driverName: 'Alex Driver',
      documentType: 'NationalId',
      documentNumber: 'ID-7788',
      pickup: 'Delhi Downtown',
      pickupLocationType: 'Domestic',
      from: '2026-08-10',
      to: '2026-08-12',
      bookedAtUtc: '2026-08-01T10:00:00Z',
      offer: {
        provider: 'BudgetWheels',
        offerId: 'offer-1',
        vehicleName: 'Swift',
        category: 'Economy',
        perDayRate: 45,
        totalPrice: 90,
        cancellationPolicy: 'Free cancellation',
        insuranceType: 'Basic',
        insuranceIncluded: true,
        currency: 'INR'
      }
    }));
    component.form.patchValue({ reference: '  BK-2024  ' });

    component.lookup();
    fixture.detectChanges();

    expect(apiService.getBookingByReference).toHaveBeenCalledWith('BK-2024');
    expect(fixture.nativeElement.textContent).toContain('BudgetWheels');
    expect(fixture.nativeElement.textContent).toContain('Alex Driver');
    expect(fixture.nativeElement.textContent).toContain('Swift / Economy');
  });

  it('renders the parsed API error when lookup fails', () => {
    apiService.getBookingByReference.and.returnValue(throwError(() => new Error('boom')));
    apiService.toApiClientError.and.returnValue({
      status: 404,
      message: 'Booking was not found.',
      validationErrors: []
    });
    component.form.patchValue({ reference: 'BK-404' });

    component.lookup();
    fixture.detectChanges();

    expect(apiService.toApiClientError).toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Booking was not found.');
  });
});