import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { Traveler } from '../../../../models/traveler';
import { TripOption } from '../../../../core/services/traveler.service';

@Component({
  selector: 'app-traveler-detail',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './traveler-detail.html',
  styleUrl: './traveler-detail.scss',
})
export class TravelerDetail {
  @Input() isOpen = false;
  @Input() traveler: Traveler | null = null;
  @Input() tripOptions: TripOption[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Traveler>();

  get assignedTrips(): TripOption[] {
    if (!this.traveler) return [];
    return this.tripOptions.filter(
      (t) => t.id !== 'all' && this.traveler!.assignedTripIds.includes(t.id),
    );
  }

  getInitials(): string {
    if (!this.traveler) return '';
    return `${this.traveler.firstName[0]}${this.traveler.lastName[0]}`.toUpperCase();
  }

  typeLabel(): string {
    if (!this.traveler) return '';
    return this.traveler.type === 'Child'
      ? `Child (${this.traveler.age}y)`
      : 'Adult';
  }
}
