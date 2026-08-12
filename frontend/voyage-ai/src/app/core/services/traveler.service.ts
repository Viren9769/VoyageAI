import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { Traveler, TravelerFormPayload, TravelerStats } from '../../models/traveler';

export interface TripOption {
  id: string;
  label: string;
}

@Injectable({ providedIn: 'root' })
export class TravelerService {
  private nextId = 7;

  private travelers: Traveler[] = [
    {
      id: 't1',
      firstName: 'Nikita',
      lastName: 'Vishwakarma',
      email: 'nikita@example.com',
      phone: '+1 (281) 555-0100',
      type: 'Adult',
      tripCount: 3,
      isPro: true,
      assignedTripIds: ['trip-1', 'trip-2', 'trip-3'],
    },
    {
      id: 't2',
      firstName: 'Rahul',
      lastName: 'Vishwakarma',
      email: 'rahul@example.com',
      phone: '+1 (832) 555-0145',
      type: 'Adult',
      tripCount: 2,
      assignedTripIds: ['trip-1', 'trip-2'],
    },
    {
      id: 't3',
      firstName: 'Priya',
      lastName: 'Sharma',
      email: 'priya@example.com',
      phone: '+1 (713) 555-0189',
      type: 'Adult',
      tripCount: 1,
      assignedTripIds: ['trip-3'],
    },
    {
      id: 't4',
      firstName: 'Aarav',
      lastName: 'Vishwakarma',
      email: 'aarav@example.com',
      phone: '+1 (281) 555-0190',
      type: 'Child',
      age: 10,
      tripCount: 2,
      assignedTripIds: ['trip-1', 'trip-2'],
    },
    {
      id: 't5',
      firstName: 'Diya',
      lastName: 'Vishwakarma',
      email: 'diya@example.com',
      phone: '+1 (281) 555-0181',
      type: 'Child',
      age: 7,
      tripCount: 2,
      assignedTripIds: ['trip-1', 'trip-2'],
    },
    {
      id: 't6',
      firstName: 'Meera',
      lastName: 'Sharma',
      email: 'meera@example.com',
      phone: '+1 (713) 555-0192',
      type: 'Adult',
      tripCount: 1,
      assignedTripIds: ['trip-3'],
    },
  ];

  private readonly tripOptions: TripOption[] = [
    { id: 'all', label: 'All Trips' },
    { id: 'trip-1', label: 'Switzerland Escape' },
    { id: 'trip-2', label: 'Bali Retreat' },
    { id: 'trip-3', label: 'Paris Getaway' },
  ];

  getTravelers(): Observable<Traveler[]> {
    return of([...this.travelers]);
  }

  getStats(): Observable<TravelerStats> {
    return of(this.computeStats());
  }

  getTripOptions(): Observable<TripOption[]> {
    return of(this.tripOptions);
  }

  addTraveler(payload: TravelerFormPayload): Observable<Traveler> {
    const traveler: Traveler = {
      ...payload,
      id: `t${this.nextId++}`,
      tripCount: payload.assignedTripIds.length,
    };
    this.travelers = [...this.travelers, traveler];
    return of(traveler);
  }

  editTraveler(id: string, payload: TravelerFormPayload): Observable<Traveler> {
    this.travelers = this.travelers.map((t) =>
      t.id === id
        ? { ...t, ...payload, tripCount: payload.assignedTripIds.length }
        : t,
    );
    return of(this.travelers.find((t) => t.id === id)!);
  }

  deleteTraveler(id: string): Observable<void> {
    this.travelers = this.travelers.filter((t) => t.id !== id);
    return of(void 0);
  }

  private computeStats(): TravelerStats {
    return {
      totalTravelers: this.travelers.length,
      adults: this.travelers.filter((t) => t.type === 'Adult').length,
      children: this.travelers.filter((t) => t.type === 'Child').length,
      upcomingTrips: 3,
    };
  }
}
