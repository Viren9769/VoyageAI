import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, combineLatest, forkJoin, map, of, switchMap, tap } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import { CreateTrip } from '../../models/create-trip';
import { TripData } from '../../models/trip';

interface GetTripResponse {
  tripId: string;
  tripName: string;
  destinationCountry: string;
  destinationCity: string;
  startDate: string;
  endDate: string;
  budget: number;
  currency: string;
  travelStyle: string;
  description?: string | null;
  coverImageUrl?: string | null;
  status: string;
}

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

  private readonly http = inject(HttpClient);
  private readonly apiUrl = ApiConfig.baseUrl + ApiConfig.trips.base;

  private tripsSubject = new BehaviorSubject<TripData[]>([]);
  private filterSubject = new BehaviorSubject<TripFilter>('All Trips');
  private sortSubject = new BehaviorSubject<string>('Recently Updated');

  trips$ = this.tripsSubject.asObservable();
  filter$ = this.filterSubject.asObservable();
  sort$ = this.sortSubject.asObservable();

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

  tripSummary = [
    { title: 'Total Trips', value: 0, icon: 'luggage', color: '#8B5CF6' },
    { title: 'Upcoming', value: 0, icon: 'calendar_month', color: '#3B82F6' },
    { title: 'Completed', value: 0, icon: 'check_circle', color: '#22C55E' },
    { title: 'Drafts', value: 0, icon: 'description', color: '#F59E0B' }
  ];

  loadTrips(): Observable<TripData[]> {
    return this.http.get<GetTripResponse[]>(this.apiUrl).pipe(
      switchMap(trips => {
        const mappedTrips = trips.map((trip, index) => this.mapTrip(trip, index + 1));

        const travelerCountRequests = mappedTrips.map(trip => {
          if (!trip.backendId) {
            return of(0);
          }

          return this.http.get<unknown[]>(`${this.apiUrl}/${trip.backendId}/travelers`).pipe(
            map(travelers => travelers.length),
            tap(count => {
              trip.travelers = count;
            })
          );
        });

        if (travelerCountRequests.length === 0) {
          return of(mappedTrips);
        }

        return forkJoin(travelerCountRequests).pipe(
          map(() => mappedTrips)
        );
      }),
      tap(trips => {
        this.tripsSubject.next(trips);
        this.tripSummary = this.buildSummary(trips);
      })
    );
  }

  getTrips(scope: 'my' | 'shared' = 'my'): Observable<TripData[]> {
    if (scope === 'shared') {
      return of([]);
    }

    return this.trips$;
  }

  setFilter(filter: TripFilter): void {
    this.filterSubject.next(filter);
  }

  setSort(sort: string): void {
    this.sortSubject.next(sort);
  }

  addTrip(request: CreateTrip): Observable<TripData> {
    return this.http.post<GetTripResponse>(this.apiUrl, request).pipe(
      map(response => this.mapTrip(response, Date.now())),
      tap(created => {
        this.tripsSubject.next([created, ...this.tripsSubject.value]);
        this.tripSummary = this.buildSummary(this.tripsSubject.value);
      })
    );
  }

  updateTrip(updatedTrip: TripData): Observable<TripData> {
    if (!updatedTrip.backendId) {
      throw new Error('Trip backend id is missing');
    }

    const payload = this.buildUpdatePayload(updatedTrip);

    return this.http.put<GetTripResponse>(`${this.apiUrl}/${updatedTrip.backendId}`, payload).pipe(
      map(response => this.mapTrip(response, updatedTrip.id)),
      tap(saved => {
        const updatedTrips = this.tripsSubject.value.map(trip => trip.id === saved.id ? saved : trip);
        this.tripsSubject.next(updatedTrips);
        this.tripSummary = this.buildSummary(updatedTrips);
      })
    );
  }

  deleteTrip(id: number): Observable<void> {
    const trip = this.tripsSubject.value.find(entry => entry.id === id);

    if (!trip?.backendId) {
      throw new Error('Trip backend id is missing');
    }

    return this.http.delete<void>(`${this.apiUrl}/${trip.backendId}`).pipe(
      tap(() => {
        const updatedTrips = this.tripsSubject.value.filter(entry => entry.id !== id);
        this.tripsSubject.next(updatedTrips);
        this.tripSummary = this.buildSummary(updatedTrips);
      })
    );
  }

  duplicateTrip(trip: TripData): Observable<TripData> {
    const request: CreateTrip = {
      tripName: `${trip.title} (Copy)`,
      destinationCountry: trip.destination,
      destinationCity: trip.destination,
      startDate: this.toApiDate(trip.startDate),
      endDate: this.toApiDate(trip.endDate),
      budget: trip.budget ?? 0,
      currency: trip.currency ?? 'USD',
      travelStyle: 'Adventure',
      description: '',
      coverImageUrl: trip.image,
      status: 'Draft'
    };

    return this.addTrip(request);
  }

  refreshSummary(): void {
    this.tripSummary = this.buildSummary(this.tripsSubject.value);
  }

  private mapTrip(response: GetTripResponse, fallbackId: number): TripData {
    const startDate = new Date(response.startDate);
    const endDate = new Date(response.endDate);
    const days = Math.max(1, Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)) + 1);

    return {
      id: fallbackId,
      backendId: response.tripId,
      title: response.tripName,
      destination: response.destinationCountry,
      image: response.coverImageUrl || 'images/default-trip.jpg',
      startDate: startDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }),
      endDate: endDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }),
      days,
      travelers: 0,
      progress: this.getProgress(response.status),
      budget: response.budget,
      currency: response.currency,
      status: this.getStatus(response.status)
    };
  }

  private buildUpdatePayload(trip: TripData): CreateTrip {
    return {
      tripName: trip.title,
      destinationCountry: trip.destination,
      destinationCity: trip.destination,
      startDate: this.toApiDate(trip.startDate),
      endDate: this.toApiDate(trip.endDate),
      budget: trip.budget ?? 0,
      currency: trip.currency ?? 'USD',
      travelStyle: 'Adventure',
      description: '',
      coverImageUrl: trip.image,
      status: trip.status
    };
  }

  private buildSummary(trips: TripData[]) {
    return [
      { title: 'Total Trips', value: trips.length, icon: 'luggage', color: '#8B5CF6' },
      { title: 'Upcoming', value: trips.filter(trip => trip.status === 'Upcoming').length, icon: 'calendar_month', color: '#3B82F6' },
      { title: 'Completed', value: trips.filter(trip => trip.status === 'Completed').length, icon: 'check_circle', color: '#22C55E' },
      { title: 'Drafts', value: trips.filter(trip => trip.status === 'Draft').length, icon: 'description', color: '#F59E0B' }
    ];
  }

  private getStatus(status: string): TripData['status'] {
    switch (status.toLowerCase()) {
      case 'active':
      case 'confirmed':
        return 'Active';
      case 'completed':
        return 'Completed';
      case 'upcoming':
      case 'planning':
        return 'Upcoming';
      default:
        return 'Draft';
    }
  }

  private getProgress(status: string): number {
    switch (status.toLowerCase()) {
      case 'completed':
        return 100;
      case 'active':
      case 'confirmed':
        return 75;
      default:
        return 0;
    }
  }

  private toApiDate(value: string): string {
    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return value;
    }

    return date.toISOString();
  }

}
