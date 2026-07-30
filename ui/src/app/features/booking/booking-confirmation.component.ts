import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BookingConfirmationResponseDto } from '../../core/models/car-rental.models';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking-confirmation.component.html',
  styleUrl: './booking-confirmation.component.css'
})
export class BookingConfirmationComponent {
  @Input({ required: true }) confirmation!: BookingConfirmationResponseDto;
}
