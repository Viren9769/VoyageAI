import { Component, Inject } from '@angular/core';

import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

import { MatProgressBarModule } from '@angular/material/progress-bar';

import { MatIconModule } from '@angular/material/icon';

import { CommonModule } from '@angular/common';

import { TripData } from '../../../../../../models/trip';

import { TripEditDialog } from '../trip-edit-dialog/trip-edit-dialog';

@Component({
  selector: 'app-trip-view-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './trip-view-dialog.html',
  styleUrl: './trip-view-dialog.scss'
})
export class TripViewDialog {

  readonly itinerary = [
    { day: 1, date: 'Jun 18', activity: 'Arrival and evening city walk' },
    { day: 2, date: 'Jun 19', activity: 'Mountain rail and old town exploration' },
    { day: 3, date: 'Jun 20', activity: 'Adventure trail and local food spots' },
    { day: 4, date: 'Jun 21', activity: 'Scenic lake cruise and photo session' }
  ];

  readonly recentActivity = [
    { text: 'Rahul Sharma uploaded a new document', time: '2h ago' },
    { text: 'Priya Patel added an expense', time: '5h ago' },
    { text: 'You updated itinerary for Day 4', time: '1d ago' },
    { text: 'John Doe booked a hotel for Day 3', time: '2d ago' }
  ];

  readonly budgetShares = [
    { name: 'Flights', share: 39 },
    { name: 'Hotels', share: 30 },
    { name: 'Food', share: 15 },
    { name: 'Activities', share: 9 },
    { name: 'Transport', share: 4 },
    { name: 'Other', share: 3 }
  ];

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public trip: TripData,

    private dialog: MatDialog,

    private dialogRef: MatDialogRef<TripViewDialog>
  ) {}

  get tripId(): string {

    return `TRP-${String(this.trip.id).padStart(6, '0')}`;

  }

  get totalBudget(): number {

    return Math.max(1500, (this.trip.days * 280) + (this.trip.travelers * 170));

  }

  get spentBudget(): number {

    return Math.round((this.totalBudget * this.trip.progress) / 100);

  }

  get remainingBudget(): number {

    return Math.max(0, this.totalBudget - this.spentBudget);

  }

  get completedDays(): number {

    return Math.max(0, Math.round((this.trip.days * this.trip.progress) / 100));

  }

  get nextEventLabel(): string {

    return this.trip.status === 'Completed' ? 'Trip Completed' : 'Tomorrow, 09:30 AM';

  }

  get airportCode(): string {

    return this.trip.destination.slice(0, 3).toUpperCase();

  }

  get weatherTemp(): string {

    return `${Math.max(18, Math.min(32, 14 + this.trip.days))}°C`;

  }

  get weatherHigh(): string {

    return `${Math.max(22, Math.min(35, 18 + this.trip.days))}°C`;

  }

  get weatherLow(): string {

    return `${Math.max(11, Math.min(22, 8 + this.trip.travelers))}°C`;

  }

  get budgetBreakdown(): Array<{ name: string; share: number; amount: number }> {

    return this.budgetShares.map(section => ({
      ...section,
      amount: Math.round((this.totalBudget * section.share) / 100)
    }));

  }

  get galleryImages(): string[] {

    return [
      this.trip.image,
      'https://images.unsplash.com/photo-1531218150217-54595bc2b934?auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1501594907352-04cda38ebc29?auto=format&fit=crop&w=800&q=80',
      'https://images.unsplash.com/photo-1476514525535-07fb3b4ae5f1?auto=format&fit=crop&w=800&q=80'
    ];

  }

  close(): void {

    this.dialogRef.close();

  }

  openEditDialog(): void {

    if (this.trip.status === 'Completed') {

      return;

    }

    // Close View Dialog
    this.dialogRef.close();

    // Open Edit Dialog
    this.dialog.open(TripEditDialog, {

      width: '1280px',

      height: '92vh',

      maxWidth: '96vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

  }

}