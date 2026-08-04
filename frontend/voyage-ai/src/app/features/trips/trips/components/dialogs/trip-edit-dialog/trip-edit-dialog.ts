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

import { MatSlideToggleModule } from '@angular/material/slide-toggle';

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
    MatSliderModule,
    MatSlideToggleModule
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

  readonly currencies = [
    'USD',
    'CHF',
    'EUR',
    'GBP'
  ];

  readonly travelStyles = [
    'Adventure',
    'Relaxed',
    'Luxury',
    'Family'
  ];

  readonly accommodationTypes = [
    '4 Star Hotels',
    'Boutique Stay',
    'Apartment',
    'Hostel'
  ];

  readonly transportationTypes = [
    'Public Transport',
    'Rental Car',
    'Private Transfer',
    'Mixed'
  ];

  constructor(

    private fb: FormBuilder,

    public dialogRef: MatDialogRef<TripEditDialog>,

    @Inject(MAT_DIALOG_DATA)

    public trip: TripData

  ) {}

  get tripId(): string {

    return `TRP-${String(this.trip.id).padStart(6, '0')}`;

  }

  get coverImage(): string {

    return this.tripForm?.get('image')?.value || this.trip.image;

  }

  get travelerAvatars(): string[] {

    return this.previewNames.slice(0, this.tripForm.get('travelers')?.value || 1);

  }

  get previewTags(): string[] {

    const tagValue = this.tripForm?.get('tags')?.value as string;

    if (!tagValue?.trim()) {

      return ['Mountains', 'Nature', 'Adventure'];

    }

    return tagValue
      .split(',')
      .map(tag => tag.trim())
      .filter(Boolean)
      .slice(0, 6);

  }

  readonly previewNames = [
    'RS',
    'PP',
    'JD',
    'MK',
    'AR',
    'SK'
  ];

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
        'We prefer mountain views and hotel near city center.'
      ],

      totalBudget: [
        Math.max(1500, (this.trip.days * 280) + (this.trip.travelers * 170)),
        [Validators.min(0)]
      ],

      currency: [
        'USD',
        Validators.required
      ],

      flightsBudget: [650],

      hotelsBudget: [500],

      foodBudget: [250],

      activitiesBudget: [350],

      transportBudget: [200],

      otherBudget: [150],

      travelStyle: [
        'Adventure',
        Validators.required
      ],

      accommodationType: [
        '4 Star Hotels',
        Validators.required
      ],

      transportationType: [
        'Public Transport',
        Validators.required
      ],

      specialRequests: [
        ''
      ],

      tags: [
        'Mountains, Nature, Adventure, Photography'
      ],

      allowTravelersEdit: [true],

      shareExpenses: [true],

      emailNotifications: [true],

      makeTripPublic: [false]

    });

    this.applyBusinessRules();

  }

  isActiveStatus(): boolean {

    return this.tripForm?.get('status')?.value === 'Active';

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