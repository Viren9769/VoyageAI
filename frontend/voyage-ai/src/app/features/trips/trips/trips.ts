import { Component, OnInit, inject } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Observable } from 'rxjs';

import { TripData } from '../../../models/trip';

import { TripService } from '../../../core/services/trip.service';

import { HeroHeader } from './components/hero-header/hero-header';

import { TripFilters } from './components/trip-filters/trip-filters';

import { TripCard } from './components/trip-card/trip-card';

import { TripSummary } from './components/trip-summary/trip-summary';

import { QuickActions } from './components/quick-actions/quick-actions';

@Component({
  selector: 'app-trips',

  standalone: true,

  imports: [
    CommonModule,
    HeroHeader,
    TripFilters,
    TripCard,
    TripSummary,
    QuickActions
  ],

  templateUrl: './trips.html',

  styleUrl: './trips.scss'
})
export class Trips implements OnInit {

  /**
   * Observable of filtered trips
   */
  filteredTrips$: Observable<TripData[]>;

  readonly tripService = inject(TripService);

  constructor() {

    this.filteredTrips$ = this.tripService.filteredTrips$;

  }

  ngOnInit(): void {
    this.tripService.loadTrips().subscribe();
  }

}