import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './review.html',
  styleUrl: './review.scss'
})
export class ReviewComponent {

  @Input({ required: true })
  tripData!: Record<string, unknown>;

  @Input()
  hasDateRangeError = false;

  @Input()
  duration = 0;

  @Input()
  fallbackImage = 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=1400';

  private requiredFieldMap: Array<{ key: string; label: string }> = [
    { key: 'tripName', label: 'Trip Name' },
    { key: 'destinationCountry', label: 'Destination Country' },
    { key: 'destinationCity', label: 'Destination City' },
    { key: 'startDate', label: 'Start Date' },
    { key: 'endDate', label: 'End Date' },
    { key: 'budget', label: 'Budget' },
    { key: 'currency', label: 'Currency' },
    { key: 'travelStyle', label: 'Travel Style' },
    { key: 'status', label: 'Status' }
  ];

  get missingRequiredFields(): string[] {

    return this.requiredFieldMap
      .filter(({ key }) => this.isMissing(key))
      .map(({ label }) => label);

  }

  private isMissing(key: string): boolean {

    const value = this.tripData?.[key];

    if (typeof value === 'number') {
      return Number.isNaN(value) || value < 0;
    }

    return value === null || value === undefined || `${value}`.trim() === '';

  }

  getTextValue(key: string, fallback = '--'): string {

    const value = this.tripData?.[key];

    if (value === null || value === undefined) {
      return fallback;
    }

    const text = `${value}`.trim();
    return text.length > 0 ? text : fallback;

  }

  getDateValue(key: string): string | number | Date | null {

    const value = this.tripData?.[key];

    if (!value) {
      return null;
    }

    if (value instanceof Date || typeof value === 'string' || typeof value === 'number') {
      return value;
    }

    return null;

  }

  getNumericValue(key: string, fallback = 0): number {

    const value = this.tripData?.[key];

    if (typeof value === 'number') {
      return value;
    }

    if (typeof value === 'string') {
      const parsed = Number(value);
      return Number.isNaN(parsed) ? fallback : parsed;
    }

    return fallback;

  }

}