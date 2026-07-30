import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ApiValidationIssueDto, SearchCarResponseDto } from '../../core/models/car-rental.models';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.css'
})
export class SearchResultsComponent {
  @Input() offers: SearchCarResponseDto[] = [];
  @Input() hasSearched = false;
  @Input() isLoading = false;
  @Input() errorMessage = '';
  @Input() validationErrors: ApiValidationIssueDto[] = [];
  @Input() selectedOfferId = '';

  @Output() readonly offerSelected = new EventEmitter<SearchCarResponseDto>();

  selectOffer(offer: SearchCarResponseDto): void {
    this.offerSelected.emit(offer);
  }
}
