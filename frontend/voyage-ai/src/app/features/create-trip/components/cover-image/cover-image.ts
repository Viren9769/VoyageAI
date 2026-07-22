import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-cover-image',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  templateUrl: './cover-image.html',
  styleUrl: './cover-image.scss'
})
export class CoverImageComponent {

  @Input({ required: true })
  form!: FormGroup;

  readonly defaultImage =
    'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=1200';

}