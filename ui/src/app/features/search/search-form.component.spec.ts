import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchFormComponent } from './search-form.component';

describe('SearchFormComponent', () => {
  let fixture: ComponentFixture<SearchFormComponent>;
  let component: SearchFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchFormComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchFormComponent);
    component = fixture.componentInstance;
    component.pickupLocations = [
      { name: 'Delhi Downtown', locationType: 'Domestic' },
      { name: 'Dubai Airport', locationType: 'International' }
    ];
    fixture.detectChanges();
  });

  it('shows validation messages and does not emit when the form is empty', () => {
    spyOn(component.searchRequested, 'emit');

    component.submit();
    fixture.detectChanges();

    expect(component.searchRequested.emit).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Pickup location is required.');
    expect(fixture.nativeElement.textContent).toContain('Pickup date is required.');
    expect(fixture.nativeElement.textContent).toContain('Return date is required.');
  });

  it('shows a date order error when return date is not after pickup date', () => {
    component.form.patchValue({
      pickup: 'Delhi Downtown',
      from: '2026-08-10',
      to: '2026-08-10'
    });

    component.submit();
    fixture.detectChanges();

    expect(component.form.hasError('dateOrder')).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('Return date must be after pickup date.');
  });

  it('emits a trimmed search request with the matched pickup location type', () => {
    spyOn(component.searchRequested, 'emit');

    component.form.patchValue({
      pickup: '  delhi downtown  ',
      from: '2026-08-10',
      to: '2026-08-12',
      category: '  '
    });

    component.submit();

    expect(component.searchRequested.emit).toHaveBeenCalledWith({
      pickup: 'delhi downtown',
      pickupLocationType: 'Domestic',
      from: '2026-08-10',
      to: '2026-08-12',
      category: undefined
    });
  });
});