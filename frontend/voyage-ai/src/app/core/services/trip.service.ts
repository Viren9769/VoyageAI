import { Injectable } from '@angular/core';

import { BehaviorSubject, combineLatest } from 'rxjs';

import { map } from 'rxjs/operators';

import { TripData } from '../../models/trip';

export type TripFilter =
  | 'All Trips'
  | 'Upcoming'
  | 'Ongoing'
  | 'Completed'
  | 'Drafts';

@Injectable({
  providedIn: 'root'
})
export class TripService {

  // ============================================================
  // Dummy Data
  // Replace with API response later
  // ============================================================

  private tripsSubject = new BehaviorSubject<TripData[]>([

    {
      id: 1,
      title: 'Switzerland Escape',
      destination: 'Switzerland',
      image: 'images/switzerland.jpg',
      startDate: 'Jun 18, 2026',
      endDate: 'Jun 26, 2026',
      days: 8,
      travelers: 4,
      progress: 75,
      status: 'Active'
    },

    {
      id: 2,
      title: 'Tokyo Explorer',
      destination: 'Japan',
      image: 'images/tokyo.jpeg',
      startDate: 'Jul 20, 2026',
      endDate: 'Jul 27, 2026',
      days: 8,
      travelers: 3,
      progress: 0,
      status: 'Upcoming'
    },

    {
      id: 3,
      title: 'Bali Adventure',
      destination: 'Indonesia',
      image: 'images/bali.jpeg',
      startDate: 'Apr 5, 2026',
      endDate: 'Apr 12, 2026',
      days: 8,
      travelers: 2,
      progress: 100,
      status: 'Completed'
    },

    {
      id: 4,
      title: 'Paris Getaway',
      destination: 'France',
      image: 'images/paris.jpeg',
      startDate: 'May 15, 2026',
      endDate: 'May 22, 2026',
      days: 7,
      travelers: 2,
      progress: 100,
      status: 'Completed'
    },

    {
      id: 5,
      title: 'Iceland Road Trip',
      destination: 'Iceland',
      image: 'images/iceland.jpeg',
      startDate: 'Sep 10, 2026',
      endDate: 'Sep 17, 2026',
      days: 7,
      travelers: 4,
      progress: 0,
      status: 'Draft'
    }

  ]);

  // ============================================================
  // Filter State
  // ============================================================

  private filterSubject =
    new BehaviorSubject<TripFilter>('All Trips');

  private sortSubject =
    new BehaviorSubject<string>('Recently Updated');

  // ============================================================
  // Public Observables
  // ============================================================

  trips$ = this.tripsSubject.asObservable();

  filter$ = this.filterSubject.asObservable();

  sort$ = this.sortSubject.asObservable();

  // ============================================================
  // Filtered Trips
  // ============================================================

  filteredTrips$ = combineLatest([
    this.trips$,
    this.filter$,
    this.sort$
  ]).pipe(

    map(([trips, filter, sort]) => {

      let filtered = [...trips];

      switch (filter) {

        case 'Upcoming':
          filtered = filtered.filter(x => x.status === 'Upcoming');
          break;

        case 'Ongoing':
          filtered = filtered.filter(x => x.status === 'Active');
          break;

        case 'Completed':
          filtered = filtered.filter(x => x.status === 'Completed');
          break;

        case 'Drafts':
          filtered = filtered.filter(x => x.status === 'Draft');
          break;

      }

      switch (sort) {

        case 'Newest':
          filtered.reverse();
          break;

        case 'Oldest':
          break;

        case 'Recently Updated':
        default:
          break;

      }

      return filtered;

    })

  );

  // ============================================================
  // Filter
  // ============================================================

  setFilter(filter: TripFilter): void {

    this.filterSubject.next(filter);

  }

  // ============================================================
  // Sort
  // ============================================================

  setSort(sort: string): void {

    this.sortSubject.next(sort);

  }

  // ============================================================
  // Load Trips
  // ============================================================

  loadTrips(trips: TripData[]): void {

    this.tripsSubject.next(trips);

  }

  // ============================================================
  // Add Trip
  // ============================================================

  addTrip(trip: TripData): void {

    this.tripsSubject.next([
      trip,
      ...this.tripsSubject.value
    ]);

  }

  // ============================================================
  // Update Trip
  // ============================================================

  updateTrip(updatedTrip: TripData): void {

    const updatedTrips = this.tripsSubject.value.map(trip =>

      trip.id === updatedTrip.id
        ? updatedTrip
        : trip

    );

    this.tripsSubject.next(updatedTrips);

  }

  // ============================================================
  // Delete Trip
  // ============================================================

  deleteTrip(id: number): void {

    const updatedTrips = this.tripsSubject.value.filter(

      trip => trip.id !== id

    );

    this.tripsSubject.next(updatedTrips);

  }

  // ============================================================
  // Duplicate Trip
  // ============================================================

  duplicateTrip(trip: TripData): void {

    const duplicatedTrip: TripData = {

      ...trip,

      id: Date.now(),

      title: `${trip.title} (Copy)`,

      status: 'Draft',

      progress: 0

    };

    this.tripsSubject.next([

      duplicatedTrip,

      ...this.tripsSubject.value

    ]);

  }

  // ============================================================
  // Dashboard Summary
  // ============================================================

  tripSummary = [

    {
      title: 'Total Trips',
      value: 18,
      icon: 'luggage',
      color: '#8B5CF6'
    },

    {
      title: 'Upcoming',
      value: 6,
      icon: 'calendar_month',
      color: '#3B82F6'
    },

    {
      title: 'Completed',
      value: 9,
      icon: 'check_circle',
      color: '#22C55E'
    },

    {
      title: 'Drafts',
      value: 3,
      icon: 'description',
      color: '#F59E0B'
    }

  ];

}