import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

import { MatMenuModule } from '@angular/material/menu';

import { MatDividerModule } from '@angular/material/divider';

import { MatProgressBarModule } from '@angular/material/progress-bar';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { TripData } from '../../../../../models/trip';

import { TripViewDialog } from '../dialogs/trip-view-dialog/trip-view-dialog';

import { TripEditDialog } from '../dialogs/trip-edit-dialog/trip-edit-dialog';

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
    MatDialogModule
  ],

  templateUrl: './trip-card.html',

  styleUrl: './trip-card.scss'
})

export class TripCard {

  @Input({ required: true })

  trip!: TripData;

  constructor(
    private dialog: MatDialog
  ) {}

  openViewDialog(): void {

    this.dialog.open(TripViewDialog, {

      width: '1100px',

      height: '85vh',

      maxWidth: '95vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

  }

  openEditDialog(): void {

    const dialogRef = this.dialog.open(TripEditDialog, {

      width: '1100px',

      height: '90vh',

      maxWidth: '95vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {

        return;

      }

      // Temporary update (until backend/service is connected)
      this.trip = result;

      console.log('Updated Trip:', result);

    });

  }

}