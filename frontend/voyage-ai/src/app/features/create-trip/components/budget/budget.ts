import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-budget',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './budget.html',
  styleUrl: './budget.scss'
})
export class BudgetComponent {

  @Input({ required: true })
  form!: FormGroup;

  currencies = [
    'USD',
    'EUR',
    'GBP',
    'INR',
    'CAD',
    'AUD',
    'JPY',
    'CHF',
    'AED',
    'SGD'
  ];

}