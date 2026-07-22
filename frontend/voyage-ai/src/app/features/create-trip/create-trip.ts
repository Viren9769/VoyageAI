import { Component, OnInit, inject } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router } from '@angular/router';

import { MatStepperModule } from '@angular/material/stepper';

import { MatButtonModule } from '@angular/material/button';

import { MatCardModule } from '@angular/material/card';

import { MatIconModule } from '@angular/material/icon';

import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { CreateTripService } from '../../core/services/create-trip.service';
import {CreateTrip} from '../../models/create-trip';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import {BasicInfoComponent} from './components/basic-info/basic-info';
import {TravelDetailsComponent} from './components/travel-details/travel-details';
import {BudgetComponent} from './components/budget/budget';
import {CoverImageComponent} from './components/cover-image/cover-image';
import {ReviewComponent} from './components/review/review';

@Component({
  selector: 'app-create-trip',

  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatSnackBarModule,
    MatChipsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    BasicInfoComponent,
    TravelDetailsComponent,
    BudgetComponent,
    CoverImageComponent,
    ReviewComponent
  ],

  templateUrl: './create-trip.html',

  styleUrl: './create-trip.scss'
})
export class CreateTripComponent implements OnInit  {

  private fb = inject(FormBuilder);

  private router = inject(Router);

  private snackBar = inject(MatSnackBar);

  private createTripService = inject(CreateTripService);

  tripForm!: FormGroup;

  duration = 0;

  loading = false;

  ngOnInit(): void {

    this.buildForm();

    this.calculateDuration();

  }

  private buildForm(): void {

    this.tripForm = this.fb.group({

      tripName: ['', Validators.required],

      destinationCountry: ['', Validators.required],

      destinationCity: ['', Validators.required],

      startDate: ['', Validators.required],

      endDate: ['', Validators.required],

      budget: [0, [Validators.required, Validators.min(0)]],

      currency: ['USD', Validators.required],

      travelStyle: ['', Validators.required],

      description: [''],

      coverImageUrl: [''],

      status: ['Draft', Validators.required]

    });

  }

  private calculateDuration(): void {

    this.tripForm.get('startDate')?.valueChanges.subscribe(() => {

      this.updateDuration();

    });

    this.tripForm.get('endDate')?.valueChanges.subscribe(() => {

      this.updateDuration();

    });

  }

  private updateDuration(): void {

    const start = this.tripForm.value.startDate;

    const end = this.tripForm.value.endDate;

    if (!start || !end) {

      this.duration = 0;

      return;

    }

    const startDate = new Date(start);

    const endDate = new Date(end);

    const diff = endDate.getTime() - startDate.getTime();

    this.duration = Math.ceil(

      diff / (1000 * 60 * 60 * 24)

    );

  }

  submit(): void {

    if (this.tripForm.invalid) {

      this.tripForm.markAllAsTouched();

      return;

    }

    this.loading = true;

    const request: CreateTrip = this.tripForm.value;

    this.createTripService.createTrip(request).subscribe({

      next: () => {

        this.loading = false;

        this.snackBar.open(

          'Trip created successfully',

          'Close',

          {

            duration: 3000

          }

        );

        this.router.navigate(['/trips']);

      },

      error: () => {

        this.loading = false;

        this.snackBar.open(

          'Unable to create trip',

          'Close',

          {

            duration: 3000

          }

        );

      }

    });

  }

  cancel(): void {

    this.router.navigate(['/trips']);

  }

}