import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CreateTrip } from '../../models/create-trip';
import { CreateTripService } from '../../core/services/create-trip.service';
import { BasicInfoComponent } from './components/basic-info/basic-info';
import { TravelDetailsComponent } from './components/travel-details/travel-details';
import { BudgetComponent } from './components/budget/budget';
import { CoverImageComponent } from './components/cover-image/cover-image';
import { ReviewComponent } from './components/review/review';

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
export class CreateTripComponent implements OnInit {

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private createTripService = inject(CreateTripService);

  tripForm!: FormGroup;
  duration = 0;
  isSubmitting = false;
  readonly previewFallbackImage = 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=1400';

  private readonly toastDuration = 3200;

  ngOnInit(): void {
    this.buildForm();
    this.calculateDuration();
  }

  private buildForm(): void {
    this.tripForm = this.fb.group({
      tripName: ['', Validators.required],
      destinationCountry: ['', Validators.required],
      destinationCity: ['', Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      budget: [0, [Validators.required, Validators.min(0)]],
      currency: ['USD', Validators.required],
      travelStyle: ['', Validators.required],
      description: [''],
      coverImageUrl: ['', Validators.pattern(/^https?:\/\/.+/i)],
      status: ['Draft', Validators.required]
    }, {
      validators: this.dateRangeValidator()
    });
  }

  private dateRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const start = control.get('startDate')?.value;
      const end = control.get('endDate')?.value;

      if (!start || !end) {
        return null;
      }

      const startDate = new Date(start);
      const endDate = new Date(end);

      if (Number.isNaN(startDate.getTime()) || Number.isNaN(endDate.getTime())) {
        return null;
      }

      return endDate >= startDate ? null : { dateRange: true };
    };
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
    this.duration = Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }

  isBasicInformationComplete(): boolean {
    return [
      'tripName',
      'destinationCountry',
      'destinationCity',
      'travelStyle',
      'status'
    ].every((field) => this.tripForm.get(field)?.valid);
  }

  isTravelDetailsComplete(): boolean {
    const hasValidDates = ['startDate', 'endDate']
      .every((field) => this.tripForm.get(field)?.valid);

    return hasValidDates && !this.tripForm.hasError('dateRange');
  }

  isBudgetComplete(): boolean {
    return ['budget', 'currency'].every((field) => this.tripForm.get(field)?.valid);
  }

  canSubmit(): boolean {
    return (
      this.tripForm.valid &&
      !this.tripForm.hasError('dateRange') &&
      this.isBasicInformationComplete() &&
      this.isTravelDetailsComplete() &&
      this.isBudgetComplete()
    );
  }

  formatPreviewDate(dateValue: Date | string | null): string {
    if (!dateValue) {
      return '--';
    }

    const date = new Date(dateValue);

    if (Number.isNaN(date.getTime())) {
      return '--';
    }

    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }

  private toDateOnlyString(dateValue: Date | string | null): string {
    if (!dateValue) {
      return '';
    }

    const date = new Date(dateValue);

    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const year = date.getFullYear();
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const day = `${date.getDate()}`.padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private buildBackendPayload(): CreateTrip {
    const rawValue = this.tripForm.getRawValue();

    return {
      tripName: (rawValue.tripName ?? '').trim(),
      destinationCountry: (rawValue.destinationCountry ?? '').trim(),
      destinationCity: (rawValue.destinationCity ?? '').trim(),
      startDate: this.toDateOnlyString(rawValue.startDate),
      endDate: this.toDateOnlyString(rawValue.endDate),
      budget: Number(rawValue.budget ?? 0),
      currency: rawValue.currency ?? 'USD',
      travelStyle: rawValue.travelStyle ?? '',
      description: (rawValue.description ?? '').trim(),
      coverImageUrl: (rawValue.coverImageUrl ?? '').trim(),
      status: rawValue.status ?? 'Draft'
    };
  }

  private showToast(
    message: string,
    panelClass: string,
    duration = this.toastDuration
  ): void {
    this.snackBar.open(message, 'Close', {
      duration,
      panelClass: [panelClass]
    });
  }

  submit(): void {
    if (!this.canSubmit()) {
      this.tripForm.markAllAsTouched();
      this.showToast(
        'Please complete all required fields before creating the trip.',
        'trip-toast-warning',
        3600
      );
      return;
    }

    this.isSubmitting = true;
    this.showToast('Creating trip...', 'trip-toast-info', 1600);

    const payload = this.buildBackendPayload();

    this.createTripService.createTrip(payload).subscribe({
      next: () => {
        this.showToast('Trip created successfully.', 'trip-toast-success', 3200);
        this.router.navigate(['/trips']);
      },
      error: () => {
        this.showToast('Trip creation failed. Please try again.', 'trip-toast-error', 4200);
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/trips']);
  }

}