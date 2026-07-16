import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

import { UpcomingTrip as UpcomingTripModel } from '../../../../models/dashboard';

@Component({
  selector: 'app-upcoming-trip',

  standalone: true,

  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule
  ],

  templateUrl: './upcoming-trip.html',

  styleUrl: './upcoming-trip.scss'
})
export class UpcomingTrip {

  @Input({ required: true })

  trip!: UpcomingTripModel;

}