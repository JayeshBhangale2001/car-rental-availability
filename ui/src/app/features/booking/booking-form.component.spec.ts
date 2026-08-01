import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BookingFormComponent } from './booking-form.component';

describe('BookingFormComponent', () => {
  let fixture: ComponentFixture<BookingFormComponent>;
  let component: BookingFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BookingFormComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(BookingFormComponent);
    component = fixture.componentInstance;
    component.selectedOffer = {
      provider: 'BudgetWheels',
      offerId: 'offer-1',
      vehicleName: 'Swift',
      category: 'Economy',
      perDayRate: 45,
      totalPrice: 90,
      cancellationPolicy: 'Free cancellation',
      insuranceIncluded: true,
      currency: 'INR'
    };
    component.searchCriteria = {
      pickup: 'Delhi Downtown',
      pickupLocationType: 'Domestic',
      from: '2026-08-10',
      to: '2026-08-12'
    };
    fixture.detectChanges();
  });

  it('renders the selected offer and expected document type helper', () => {
    expect(fixture.nativeElement.textContent).toContain('BudgetWheels');
    expect(fixture.nativeElement.textContent).toContain('Swift');
    expect(fixture.nativeElement.textContent).toContain('Expected for this pickup: NationalId');
  });

  it('shows validation messages and does not emit when required fields are missing', () => {
    spyOn(component.bookingRequested, 'emit');

    component.submit();
    fixture.detectChanges();

    expect(component.bookingRequested.emit).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Driver name is required.');
    expect(fixture.nativeElement.textContent).toContain('Document type is required.');
    expect(fixture.nativeElement.textContent).toContain('Document number is required.');
  });

  it('shows a local error when the document type does not match the pickup rules', () => {
    spyOn(component.bookingRequested, 'emit');
    component.form.patchValue({
      driverName: 'Alex Driver',
      documentType: 'Passport',
      documentNumber: 'P12345'
    });

    component.submit();
    fixture.detectChanges();

    expect(component.bookingRequested.emit).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('For Delhi Downtown, document type must be NationalId.');
  });

  it('emits a trimmed booking request when the form is valid', () => {
    spyOn(component.bookingRequested, 'emit');
    component.form.patchValue({
      driverName: '  Alex Driver  ',
      documentType: 'NationalId',
      documentNumber: '  ID-7788  '
    });

    component.submit();

    expect(component.bookingRequested.emit).toHaveBeenCalledWith({
      provider: 'BudgetWheels',
      offerId: 'offer-1',
      driverName: 'Alex Driver',
      documentType: 'NationalId',
      documentNumber: 'ID-7788',
      pickup: 'Delhi Downtown',
      from: '2026-08-10',
      to: '2026-08-12'
    });
  });
});