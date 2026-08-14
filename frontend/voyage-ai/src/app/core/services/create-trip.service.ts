import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

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

@Injectable({
  providedIn: 'root'
})
export class CreateTripService {

  private http = inject(HttpClient);

  private readonly apiUrl = ApiConfig.baseUrl + ApiConfig.trips.base;

  constructor() { }

  // ============================================================
  // Create Trip
  // POST: /api/trips
  // ============================================================

  createTrip(request: CreateTrip): Observable<TripData> {

    return this.http.post<GetTripResponse>(this.apiUrl, request).pipe(
      map(response => this.mapTrip(response, 0))
    );

  }

  // ============================================================
  // Get All Trips
  // GET: /api/trips
  // ============================================================

  getTrips(): Observable<TripData[]> {

    return this.http.get<GetTripResponse[]>(this.apiUrl).pipe(
      map(trips => trips.map((trip, index) => this.mapTrip(trip, index + 1)))
    );

  }

  // ============================================================
  // Get Trip By Id
  // GET: /api/trips/{id}
  // ============================================================

  getTrip(id: number): Observable<TripData> {

    return this.getTrips().pipe(
      map(trips => trips.find(trip => trip.id === id) as TripData)
    );

  }

  // ============================================================
  // Update Trip
  // PUT: /api/trips/{id}
  // ============================================================

  updateTrip(
    id: number,
    request: CreateTrip
  ): Observable<TripData> {

    return this.http.put<GetTripResponse>(`${this.apiUrl}/${id}`, request).pipe(
      map(response => this.mapTrip(response, id))
    );

  }

  // ============================================================
  // Delete Trip
  // DELETE: /api/trips/{id}
  // ============================================================

  deleteTrip(id: number): Observable<void> {

    return this.http.delete<void>(`${this.apiUrl}/${id}`);

  }

  private mapTrip(response: GetTripResponse, fallbackId: number): TripData {
    const startDate = new Date(response.startDate);
    const endDate = new Date(response.endDate);
    const days = Math.max(1, Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)) + 1);

    return {
      id: fallbackId || Date.now(),
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
      case 'upcoming':
      case 'planning':
        return 0;
      default:
        return 0;
    }
  }

}