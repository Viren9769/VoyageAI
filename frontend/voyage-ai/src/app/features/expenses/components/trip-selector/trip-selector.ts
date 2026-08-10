import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { TripExpenseContext } from '../../../../models/expense';

@Component({
  selector: 'app-trip-selector',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './trip-selector.html',
  styleUrl: './trip-selector.scss',
})
export class TripSelector {
  @Input() trips: TripExpenseContext[] = [];
  @Input() selectedTripId = '';

  @Output() selectedTripIdChange = new EventEmitter<string>();

  onTripChange(nextId: string): void {
    this.selectedTripIdChange.emit(nextId);
  }

  statusClass(status: TripExpenseContext['status']): string {
    return status.toLowerCase();
  }
}
