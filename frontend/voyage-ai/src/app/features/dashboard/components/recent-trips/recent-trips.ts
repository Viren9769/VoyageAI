import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { RecentTrip } from '../../../../models/dashboard';

@Component({
  selector: 'app-recent-trips',

  standalone: true,

  imports: [
    CommonModule
  ],

  templateUrl: './recent-trips.html',

  styleUrl: './recent-trips.scss'
})
export class RecentTrips {

  @Input({ required: true })

  trips!: RecentTrip[];

}