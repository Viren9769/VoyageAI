import { Component, Inject, inject } from '@angular/core';

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
import { forkJoin } from 'rxjs';

import { TripData } from '../../../../../../models/trip';

import { TripEditDialog } from '../trip-edit-dialog/trip-edit-dialog';
import { ItineraryService } from '../../../../../../core/services/itinerary.service';
import { ExpenseService } from '../../../../../../core/services/expense.service';

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

  private readonly itineraryService = inject(ItineraryService);
  private readonly expenseService = inject(ExpenseService);

  itinerary: Array<{ day: number; date: string; activity: string }> = [];

  recentActivity: Array<{ text: string; time: string }> = [];

  budgetShares: Array<{ name: string; share: number }> = [];

  private totalBudgetValue?: number;
  private spentBudgetValue?: number;
  private remainingBudgetValue?: number;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public trip: TripData,

    private dialog: MatDialog,

    private dialogRef: MatDialogRef<TripViewDialog>
  ) {
    if (this.trip.backendId) {
      this.loadLiveData(this.trip.backendId);
    }
  }

  get tripId(): string {

    return `TRP-${String(this.trip.id).padStart(6, '0')}`;

  }

  get totalBudget(): number {

    return this.totalBudgetValue ?? this.trip.budget ?? Math.max(1500, (this.trip.days * 280) + (this.trip.travelers * 170));

  }

  get spentBudget(): number {

    return this.spentBudgetValue ?? Math.round((this.totalBudget * this.trip.progress) / 100);

  }

  get remainingBudget(): number {

    return this.remainingBudgetValue ?? Math.max(0, this.totalBudget - this.spentBudget);

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

  private loadLiveData(tripId: string): void {
    forkJoin({
      days: this.itineraryService.getTripDays(tripId),
      expenses: this.expenseService.getDashboardData(tripId),
    }).subscribe({
      next: ({ days, expenses }) => {
        this.itinerary = days.slice(0, 5).map(day => ({
          day: day.dayNumber,
          date: day.date,
          activity: day.title || day.theme,
        }));

        this.budgetShares = expenses.categories
          .map(category => {
            const share = this.totalBudget > 0
              ? Math.round((category.amount / this.totalBudget) * 100)
              : 0;

            return {
              name: category.category,
              share,
            };
          })
          .filter(item => item.share > 0);

        this.totalBudgetValue = expenses.summary.totalBudget;
        this.spentBudgetValue = expenses.summary.totalSpent;
        this.remainingBudgetValue = expenses.summary.remaining;

        this.recentActivity = expenses.expenses
          .slice(0, 4)
          .map(expense => ({
            text: `${expense.description} (${expense.category})`,
            time: expense.date,
          }));
      },
      error: () => {
        this.itinerary = [];
        this.budgetShares = [];
        this.recentActivity = [];
      }
    });
  }

}