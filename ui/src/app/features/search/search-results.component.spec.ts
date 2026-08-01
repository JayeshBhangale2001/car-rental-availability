import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchResultsComponent } from './search-results.component';

describe('SearchResultsComponent', () => {
  let fixture: ComponentFixture<SearchResultsComponent>;
  let component: SearchResultsComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchResultsComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchResultsComponent);
    component = fixture.componentInstance;
  });

  it('renders offers and highlights the selected one', () => {
    component.offers = [
      {
        provider: 'BudgetWheels',
        offerId: 'offer-1',
        vehicleName: 'Swift',
        category: 'Economy',
        perDayRate: 45,
        totalPrice: 90,
        cancellationPolicy: 'Free cancellation',
        insuranceIncluded: true,
        currency: 'INR'
      }
    ];
    component.selectedOfferId = 'offer-1';

    fixture.detectChanges();

    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(fixture.nativeElement.textContent).toContain('BudgetWheels');
    expect(fixture.nativeElement.textContent).toContain('Swift');
    expect(button.textContent).toContain('Selected');
    expect(button.classList).toContain('active');
  });

  it('emits the selected offer when the button is clicked', () => {
    const offer = {
      provider: 'PremiumDrive',
      offerId: 'offer-2',
      vehicleName: 'Fortuner',
      category: 'SUV',
      perDayRate: 120,
      totalPrice: 360,
      cancellationPolicy: 'Non-refundable',
      insuranceIncluded: false,
      currency: 'USD'
    };
    spyOn(component.offerSelected, 'emit');
    component.offers = [offer];

    fixture.detectChanges();
    fixture.nativeElement.querySelector('button').click();

    expect(component.offerSelected.emit).toHaveBeenCalledWith(offer);
  });

  it('renders the API error state with validation details', () => {
    component.errorMessage = 'Please fix the highlighted validation errors.';
    component.validationErrors = [
      { kind: 'Validation', field: 'from', code: 'InvalidDate', message: 'Pickup date is invalid.' }
    ];

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Please fix the highlighted validation errors.');
    expect(fixture.nativeElement.textContent).toContain('from: Pickup date is invalid.');
  });
});