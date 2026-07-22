import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatDatepickerModule } from '@angular/material/datepicker';

import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-travel-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './travel-details.html',
  styleUrl: './travel-details.scss'
})
export class TravelDetailsComponent {

  @Input({ required: true })
  form!: FormGroup;

}