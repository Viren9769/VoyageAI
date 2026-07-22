import { Component, Inject, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

import { MatSelectModule } from '@angular/material/select';

import { MatDatepickerModule } from '@angular/material/datepicker';

import { MatNativeDateModule } from '@angular/material/core';

import { MatSliderModule } from '@angular/material/slider';

import { TripData } from '../../../../../../models/trip';

@Component({
  selector: 'app-trip-edit-dialog',

  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSliderModule
  ],

  templateUrl: './trip-edit-dialog.html',

  styleUrl: './trip-edit-dialog.scss'
})

export class TripEditDialog implements OnInit {

  tripForm!: FormGroup;

  readonly statuses = [
    'Draft',
    'Upcoming',
    'Active',
    'Completed'
  ];

  constructor(

    private fb: FormBuilder,

    public dialogRef: MatDialogRef<TripEditDialog>,

    @Inject(MAT_DIALOG_DATA)

    public trip: TripData

  ) {}

  ngOnInit(): void {

    this.tripForm = this.fb.group({

      title: [
        this.trip.title,
        Validators.required
      ],

      destination: [
        this.trip.destination,
        Validators.required
      ],

      image: [
        this.trip.image
      ],

      startDate: [
        this.trip.startDate,
        Validators.required
      ],

      endDate: [
        this.trip.endDate,
        Validators.required
      ],

      travelers: [
        this.trip.travelers,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],

      status: [
        this.trip.status,
        Validators.required
      ],

      progress: [
        this.trip.progress
      ],

      notes: [
        ''
      ]

    });

    this.applyBusinessRules();

  }

  private applyBusinessRules(): void {

    switch (this.trip.status) {

      case 'Active':

        this.tripForm.get('destination')?.disable();

        this.tripForm.get('startDate')?.disable();

        break;

      case 'Completed':

        this.tripForm.disable();

        this.tripForm.get('notes')?.enable();

        break;

    }

  }

  save(): void {

    if (this.tripForm.invalid) {

      this.tripForm.markAllAsTouched();

      return;

    }

    const updatedTrip: TripData = {

      ...this.trip,

      ...this.tripForm.getRawValue()

    };

    this.dialogRef.close(updatedTrip);

  }

  close(): void {

    this.dialogRef.close();

  }

}