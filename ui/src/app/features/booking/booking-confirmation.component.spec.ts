import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BookingConfirmationComponent } from './booking-confirmation.component';

describe('BookingConfirmationComponent', () => {
  let fixture: ComponentFixture<BookingConfirmationComponent>;
  let component: BookingConfirmationComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BookingConfirmationComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(BookingConfirmationComponent);
    component = fixture.componentInstance;
    component.confirmation = {
      reference: 'BK-1001',
      provider: 'PremiumDrive',
      category: 'SUV',
      totalPrice: 12500,
      cancellationPolicy: 'Free cancellation'
    };
    fixture.detectChanges();
  });

  it('renders the confirmation details', () => {
    expect(fixture.nativeElement.textContent).toContain('Booking Confirmed');
    expect(fixture.nativeElement.textContent).toContain('BK-1001');
    expect(fixture.nativeElement.textContent).toContain('PremiumDrive');
    expect(fixture.nativeElement.textContent).toContain('SUV');
    expect(fixture.nativeElement.textContent).toContain('INR 12,500.00');
  });
});