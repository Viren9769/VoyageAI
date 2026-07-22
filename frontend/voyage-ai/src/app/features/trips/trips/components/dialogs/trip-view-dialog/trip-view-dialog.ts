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

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public trip: TripData,

    private dialog: MatDialog,

    private dialogRef: MatDialogRef<TripViewDialog>
  ) {}

  close(): void {

    this.dialogRef.close();

  }

  openEditDialog(): void {

    // Close View Dialog
    this.dialogRef.close();

    // Open Edit Dialog
    this.dialog.open(TripEditDialog, {

      width: '1100px',

      height: '90vh',

      maxWidth: '95vw',

      autoFocus: false,

      panelClass: 'trip-dialog',

      data: this.trip

    });

  }

}