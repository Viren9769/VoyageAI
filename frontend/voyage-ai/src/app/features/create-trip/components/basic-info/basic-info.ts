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
  selector: 'app-basic-info',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './basic-info.html',
  styleUrl: './basic-info.scss'
})
export class BasicInfoComponent {

  @Input({ required: true })
  form!: FormGroup;

  travelStyles = [
    'Luxury',
    'Adventure',
    'Family',
    'Romantic',
    'Solo',
    'Business',
    'Road Trip',
    'Backpacking',
    'Cruise',
    'Camping'
  ];

  tripStatus = [
    'Draft',
    'Upcoming',
    'Active',
    'Completed'
  ];

}