import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';



export interface TripSummaryItem {

  title: string;

  value: number;

  icon: string;

  color: string;

}

@Component({

  selector: 'app-trip-summary',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule
  ],

  templateUrl: './trip-summary.html',

  styleUrl: './trip-summary.scss'

})

export class TripSummary {

  @Input({ required: true })

  summary!: TripSummaryItem[];

}