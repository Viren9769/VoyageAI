import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormGroup } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './review.html',
  styleUrl: './review.scss'
})
export class ReviewComponent {

  @Input({ required: true })
  form!: FormGroup;

  @Input()
  duration = 0;

}