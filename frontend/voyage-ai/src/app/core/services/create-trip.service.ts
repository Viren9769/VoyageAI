import { Injectable, inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { CreateTrip } from '../../models/create-trip';

import { TripData } from '../../models/trip';

@Injectable({
  providedIn: 'root'
})
export class CreateTripService {

  private http = inject(HttpClient);

  /**
   * Change this to your backend URL
   * Example:
   * http://localhost:5000/api
   */
  private readonly apiUrl = 'https://localhost:5001/api/trips';

  constructor() { }

  // ============================================================
  // Create Trip
  // POST: /api/trips
  // ============================================================

  createTrip(request: CreateTrip): Observable<any> {

    return this.http.post<any>(

      this.apiUrl,

      request

    );

  }

  // ============================================================
  // Get All Trips
  // GET: /api/trips
  // ============================================================

  getTrips(): Observable<TripData[]> {

    return this.http.get<TripData[]>(

      this.apiUrl

    );

  }

  // ============================================================
  // Get Trip By Id
  // GET: /api/trips/{id}
  // ============================================================

  getTrip(id: number): Observable<TripData> {

    return this.http.get<TripData>(

      `${this.apiUrl}/${id}`

    );

  }

  // ============================================================
  // Update Trip
  // PUT: /api/trips/{id}
  // ============================================================

  updateTrip(
    id: number,
    request: CreateTrip
  ): Observable<any> {

    return this.http.put<any>(

      `${this.apiUrl}/${id}`,

      request

    );

  }

  // ============================================================
  // Delete Trip
  // DELETE: /api/trips/{id}
  // ============================================================

  deleteTrip(id: number): Observable<void> {

    return this.http.delete<void>(

      `${this.apiUrl}/${id}`

    );

  }

}