import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Observable } from 'rxjs';

import { TripData } from '../../../models/trip';

import { TripService } from '../../../core/services/trip.service';

import { HeroHeader } from './components/hero-header/hero-header';

import { TripFilters } from './components/trip-filters/trip-filters';

import { TripCard } from './components/trip-card/trip-card';

import { TripSummary } from './components/trip-summary/trip-summary';

import { QuickActions } from './components/quick-actions/quick-actions';

import {TripViewDialog} from './components/dialogs/trip-view-dialog/trip-view-dialog';

import {TripEditDialog} from './components/dialogs/trip-edit-dialog/trip-edit-dialog';

@Component({
  selector: 'app-trips',

  standalone: true,

  imports: [
    CommonModule,
    HeroHeader,
    TripFilters,
    TripCard,
    TripSummary,
    QuickActions,
    TripViewDialog,
    TripEditDialog
  ],

  templateUrl: './trips.html',

  styleUrl: './trips.scss'
})
export class Trips {

  /**
   * Observable of filtered trips
   */
  filteredTrips$: Observable<TripData[]>;

  constructor(
    public tripService: TripService
  ) {

    this.filteredTrips$ = this.tripService.filteredTrips$;

  }

}