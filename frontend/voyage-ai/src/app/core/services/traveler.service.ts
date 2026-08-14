import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map, of, switchMap, tap } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import { Traveler, TravelerFormPayload, TravelerStats } from '../../models/traveler';

interface GetTripResponse {
  tripId: string;
  tripName: string;
}

interface TravelerResponse {
  travelerId: string;
  tripId: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: string | null;
  gender?: string | null;
  email?: string | null;
  phone?: string | null;
  nationality?: string | null;
  passportNumber?: string | null;
  passportCountry?: string | null;
  passportExpiry?: string | null;
  emergencyContactName?: string | null;
  emergencyContactPhone?: string | null;
  relationship?: string | null;
  dietaryPreference?: string | null;
  specialRequirements?: string | null;
  frequentFlyerNumber?: string | null;
  knownTravelerNumber?: string | null;
  isPrimaryTraveler: boolean;
  age?: number | null;
}

export interface TripOption {
  id: string;
  label: string;
}

@Injectable({ providedIn: 'root' })
export class TravelerService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = ApiConfig.baseUrl + ApiConfig.trips.base;

  private travelers: Traveler[] = [];
  private tripOptions: TripOption[] = [];

  loadData(): Observable<{ travelers: Traveler[]; stats: TravelerStats; tripOptions: TripOption[] }> {
    return this.http.get<GetTripResponse[]>(this.apiUrl).pipe(
      switchMap(trips => {
        this.tripOptions = [{ id: 'all', label: 'All Trips' }, ...trips.map(trip => ({ id: trip.tripId, label: trip.tripName }))];

        const travelerRequests = trips.map(trip =>
          this.http.get<TravelerResponse[]>(`${this.apiUrl}/${trip.tripId}/travelers`).pipe(
            map(travelers => travelers.map(traveler => this.mapTraveler(traveler, trip.tripId)))
          )
        );

        if (travelerRequests.length === 0) {
          this.travelers = [];
          const stats = this.computeStats();
          return of({ travelers: [], stats, tripOptions: [...this.tripOptions] });
        }

        return forkJoin(travelerRequests).pipe(
          map(results => results.flat()),
          tap(travelers => {
            this.travelers = travelers;
          }),
          map(travelers => ({
            travelers,
            stats: this.computeStats(),
            tripOptions: [...this.tripOptions]
          }))
        );
      })
    );
  }

  getTravelers(): Observable<Traveler[]> {
    return of([...this.travelers]);
  }

  getStats(): Observable<TravelerStats> {
    return of(this.computeStats());
  }

  getTripOptions(): Observable<TripOption[]> {
    return of([...this.tripOptions]);
  }

  addTraveler(payload: TravelerFormPayload): Observable<Traveler> {
    const tripId = this.resolveTripId(payload.assignedTripIds);
    const request = this.mapPayload(payload);

    return this.http.post<TravelerResponse>(`${this.apiUrl}/${tripId}/travelers`, request).pipe(
      map(traveler => this.mapTraveler(traveler, tripId)),
      tap(created => {
        this.travelers = [created, ...this.travelers];
      })
    );
  }

  editTraveler(id: string, payload: TravelerFormPayload): Observable<Traveler> {
    const existing = this.travelers.find(traveler => traveler.id === id);
    const tripId = existing?.assignedTripIds[0] ?? this.resolveTripId(payload.assignedTripIds);
    const request = this.mapPayload(payload);

    return this.http.put<TravelerResponse>(`${this.apiUrl}/${tripId}/travelers/${id}`, request).pipe(
      map(traveler => this.mapTraveler(traveler, tripId)),
      tap(updated => {
        this.travelers = this.travelers.map(traveler => traveler.id === id ? updated : traveler);
      })
    );
  }

  deleteTraveler(id: string): Observable<void> {
    const existing = this.travelers.find(traveler => traveler.id === id);
    const tripId = existing?.assignedTripIds[0] ?? this.tripOptions.find(option => option.id !== 'all')?.id;

    if (!tripId) {
      return of(void 0);
    }

    return this.http.delete<void>(`${this.apiUrl}/${tripId}/travelers/${id}`).pipe(
      tap(() => {
        this.travelers = this.travelers.filter(traveler => traveler.id !== id);
      })
    );
  }

  private mapTraveler(response: TravelerResponse, tripId: string): Traveler {
    return {
      id: response.travelerId,
      firstName: response.firstName,
      lastName: response.lastName,
      email: response.email ?? '',
      phone: response.phone ?? '',
      type: response.age && response.age < 18 ? 'Child' : 'Adult',
      age: response.age ?? undefined,
      tripCount: 1,
      avatarUrl: undefined,
      isPro: response.isPrimaryTraveler,
      assignedTripIds: [tripId]
    };
  }

  private mapPayload(payload: TravelerFormPayload) {
    const request: {
      firstName: string;
      lastName: string;
      email: string;
      phone: string;
      isPrimaryTraveler: boolean;
      dateOfBirth?: string;
    } = {
      firstName: payload.firstName,
      lastName: payload.lastName,
      email: payload.email,
      phone: payload.phone,
      isPrimaryTraveler: false
    };

    if (payload.type === 'Child' && typeof payload.age === 'number') {
      const birthYear = new Date().getFullYear() - payload.age;
      request.dateOfBirth = new Date(birthYear, 0, 1).toISOString();
    }

    return request;
  }

  private resolveTripId(assignedTripIds: string[]): string {
    const selected = assignedTripIds.find(id => id && id !== 'all');
    if (selected) {
      return selected;
    }

    const firstTrip = this.tripOptions.find(option => option.id !== 'all');
    if (firstTrip) {
      return firstTrip.id;
    }

    throw new Error('No trip available for traveler operation');
  }

  private computeStats(): TravelerStats {
    const totalTravelers = this.travelers.length;
    const children = this.travelers.filter(traveler => traveler.type === 'Child').length;
    const adults = totalTravelers - children;
    const upcomingTrips = Math.max(0, this.tripOptions.length - 1);

    return {
      totalTravelers,
      adults,
      children,
      upcomingTrips,
    };
  }
}
