import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

import { MatMenuModule } from '@angular/material/menu';

import { MatDividerModule } from '@angular/material/divider';

import { MatProgressBarModule } from '@angular/material/progress-bar';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TripData } from '../../../../../models/trip';

import { TripViewDialog } from '../dialogs/trip-view-dialog/trip-view-dialog';

import { TripEditDialog } from '../dialogs/trip-edit-dialog/trip-edit-dialog';

import { TripService } from '../../../../../core/services/trip.service';

@Component({
  selector: 'app-trip-card',

  standalone: true,

  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    MatProgressBarModule,
    MatDialogModule,
    MatSnackBarModule
  ],

  templateUrl: './trip-card.html',

  styleUrl: './trip-card.scss'
})

export class TripCard {

  @Input({ required: true })

  trip!: TripData;

  constructor(
    private dialog: MatDialog,
    private tripService: TripService,
    private snackBar: MatSnackBar
  ) {}

  openViewDialog(): void {

    this.dialog.open(TripViewDialog, {

      width: '1280px',

      height: '92vh',

      maxWidth: '96vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

  }

  openEditDialog(): void {

    if (this.trip.status === 'Completed') {

      return;

    }

    const dialogRef = this.dialog.open(TripEditDialog, {

      width: '1280px',

      height: '92vh',

      maxWidth: '96vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {

        return;

      }

      this.tripService.updateTrip({ ...result, backendId: this.trip.backendId }).subscribe({
        next: saved => {
          this.trip = saved;
          this.snackBar.open('Trip updated successfully.', 'Close', { duration: 2500 });
        },
        error: () => {
          this.snackBar.open('Unable to update trip right now.', 'Close', { duration: 3500 });
        }
      });

    });

  }

  deleteTrip(): void {
    if (!this.trip.backendId) {
      this.snackBar.open('Trip backend id is missing.', 'Close', { duration: 2500 });
      return;
    }

    if (!window.confirm(`Delete ${this.trip.title}?`)) {
      return;
    }

    this.tripService.deleteTrip(this.trip.id).subscribe({
      next: () => this.snackBar.open('Trip deleted successfully.', 'Close', { duration: 2500 }),
      error: () => this.snackBar.open('Unable to delete trip right now.', 'Close', { duration: 3500 })
    });
  }

}