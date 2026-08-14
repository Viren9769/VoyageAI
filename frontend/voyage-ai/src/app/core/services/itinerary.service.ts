import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map, of, switchMap, tap } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import { Trip } from '../../models/trip';
import { TripDay } from '../../models/trip-day';
import { Activity } from '../../models/activity';
import { TravelTip } from '../../models/travel-tip';

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

interface ItinerarySummaryResponse {
  dayId: string;
  tripId: string;
  dayNumber: number;
  date: string;
  title: string;
  summary: string;
  estimatedBudget: number;
  actualBudget: number;
  weatherSummary: string;
  updatedAt: string;
  isDeleted: boolean;
}

interface ItineraryDayResponse extends ItinerarySummaryResponse {
  notes: string;
  createdAt: string;
  createdBy: string;
  lastModifiedBy: string;
  deletedAt: string | null;
}

interface ActivityResponse {
  activityId: string;
  dayId: string;
  activityName: string;
  category: string;
  startTime: string;
  endTime: string;
  priority: number;
  status: number;
  estimatedCost: number;
  actualCost: number;
  locationName: string;
  description: string;
  bookingReference: string;
  updatedAt: string;
  isDeleted: boolean;
}

interface TravelTipResponse {
  id: string;
  tripId: string;
  dayNumber: number;
  title: string;
  description: string;
  imageUrl: string;
  temperatureCelsius: number;
  icon: string;
  recommendedWindow: string;
  source: string;
  aiConfidence: number;
}

@Injectable({
  providedIn: 'root'
})
export class ItineraryService {

  private readonly http = inject(HttpClient);
  private readonly apiUrl = ApiConfig.baseUrl + ApiConfig.trips.base;
  private readonly dayLookup = new Map<string, TripDay[]>();
  private readonly dayIdLookup = new Map<string, Map<number, string>>();

  getTrips(scope: 'my' | 'shared' = 'my'): Observable<Trip[]> {
    if (scope === 'shared') {
      return of([]);
    }

    return this.http.get<GetTripResponse[]>(this.apiUrl).pipe(
      map(trips => trips.map((trip, index) => this.mapTrip(trip, index + 1)))
    );
  }

  getTripDays(tripId: string): Observable<TripDay[]> {
    return this.http.get<{ data?: ItinerarySummaryResponse[] } | ItinerarySummaryResponse[]>(`${this.apiUrl}/${tripId}/itinerary`).pipe(
      map(response => {
        const source = Array.isArray(response) ? response : (response.data ?? []);
        const mapped = source.map(day => this.mapTripDay(day));

        return { source, mapped };
      }),
      tap(({ source, mapped }) => {
        this.dayLookup.set(tripId, mapped);

        const idByDayNumber = new Map<number, string>();
        for (const day of source) {
          idByDayNumber.set(day.dayNumber, day.dayId);
        }

        this.dayIdLookup.set(tripId, idByDayNumber);
      }),
      map(({ mapped }) => mapped)
    );
  }

  getActivities(tripId: string, dayNumber: number): Observable<Activity[]> {
    const dayId = this.findDayId(tripId, dayNumber);

    if (!dayId) {
      return of([]);
    }

    return this.http.get<ActivityResponse[]>(`${this.apiUrl}/${tripId}/itinerary/${dayId}/activities`).pipe(
      map(activities => activities.map(activity => this.mapActivity(activity, tripId, dayNumber)))
    );
  }

  getTravelTip(tripId: string, dayNumber: number): Observable<TravelTip> {
    return this.http.get<{ data?: TravelTipResponse } | TravelTipResponse>(`${this.apiUrl}/${tripId}/itinerary/day/${dayNumber}/travel-tip`).pipe(
      map(response => Array.isArray(response as unknown[]) ? null : (('data' in (response as { data?: TravelTipResponse }) ? (response as { data?: TravelTipResponse }).data : response) as TravelTipResponse | null)),
      map(response => response ? this.mapTravelTip(response) : this.defaultTip(tripId, dayNumber))
    );
  }

  private findDayId(tripId: string, dayNumber: number): string | undefined {
    return this.dayIdLookup.get(tripId)?.get(dayNumber);
  }

  private mapTrip(response: GetTripResponse, fallbackId: number): Trip {
    return {
      id: response.tripId,
      backendId: response.tripId,
      name: response.tripName,
      destinationCountry: response.destinationCountry,
      destinationCity: response.destinationCity,
      coverImageUrl: response.coverImageUrl || 'images/default-trip.jpg',
      startDateUtc: new Date(response.startDate).toISOString(),
      endDateUtc: new Date(response.endDate).toISOString(),
      totalDays: this.calculateDays(response.startDate, response.endDate),
      travelerCount: 0,
      budgetCurrency: response.currency,
      budgetAmount: response.budget,
      status: this.normalizeStatus(response.status),
      lastUpdatedUtc: new Date().toISOString()
    };
  }

  private mapTripDay(response: ItinerarySummaryResponse): TripDay {
    return {
      id: this.guidToNumeric(response.dayId),
      tripId: response.tripId,
      dayNumber: response.dayNumber,
      title: response.title,
      date: new Date(response.date).toLocaleDateString('en-GB', { day: '2-digit', month: 'short' }),
      theme: response.summary,
      isTransitDay: /arrival|transit/i.test(response.title),
      notes: response.summary
    };
  }

  private mapActivity(response: ActivityResponse, tripId: string, dayNumber: number): Activity {
    return {
      id: this.guidToNumeric(response.activityId),
      tripId,
      dayNumber,
      title: response.activityName,
      description: response.description,
      locationName: response.locationName,
      startTime: response.startTime,
      endTime: response.endTime,
      time: response.startTime,
      icon: this.activityIcon(response.category),
      status: this.activityStatus(response.status),
      category: this.activityCategory(response.category),
      estimatedCost: response.estimatedCost,
      currency: 'USD',
      confirmationCode: response.bookingReference || undefined
    };
  }

  private mapTravelTip(response: TravelTipResponse): TravelTip {
    return {
      id: this.guidToNumeric(response.id),
      tripId: response.tripId,
      dayNumber: response.dayNumber,
      title: response.title,
      description: response.description,
      imageUrl: response.imageUrl,
      temperatureCelsius: response.temperatureCelsius,
      icon: response.icon,
      recommendedWindow: response.recommendedWindow,
      source: response.source,
      aiConfidence: response.aiConfidence
    };
  }

  private defaultTip(tripId: string, dayNumber: number): TravelTip {
    return {
      id: 0,
      tripId,
      dayNumber,
      title: 'Travel Recommendation',
      description: 'No specific recommendation is available for this day yet. Focus on keeping transitions and transport times flexible.',
      imageUrl: 'https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=800',
      temperatureCelsius: 22,
      icon: 'auto_awesome',
      recommendedWindow: 'Anytime',
      source: 'Voyage AI Assistant',
      aiConfidence: 0.7
    };
  }

  private calculateDays(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.max(1, Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1);
  }

  private normalizeStatus(status: string): Trip['status'] {
    switch (status.toLowerCase()) {
      case 'active':
      case 'confirmed':
        return 'Active';
      case 'completed':
        return 'Completed';
      case 'planning':
      case 'upcoming':
        return 'Upcoming';
      default:
        return 'Draft';
    }
  }

  private activityCategory(category: string): Activity['category'] {
    switch (category.toLowerCase()) {
      case 'flight':
        return 'Flight';
      case 'hotel':
        return 'Hotel';
      case 'restaurant':
      case 'food':
        return 'Food';
      case 'transportation':
      case 'transport':
        return 'Transport';
      case 'museum':
      case 'sightseeing':
      case 'activity':
        return 'Activity';
      case 'shopping':
        return 'Shopping';
      default:
        return 'Other';
    }
  }

  private activityStatus(status: number): Activity['status'] {
    switch (status) {
      case 0:
        return 'Pending';
      case 1:
        return 'Confirmed';
      default:
        return 'Suggested';
    }
  }

  private activityIcon(category: string): string {
    switch (category.toLowerCase()) {
      case 'flight':
        return 'flight_takeoff';
      case 'hotel':
        return 'hotel';
      case 'restaurant':
      case 'food':
        return 'restaurant';
      case 'transportation':
      case 'transport':
        return 'directions_bus';
      case 'museum':
        return 'museum';
      default:
        return 'explore';
    }
  }

  private guidToNumeric(value: string): number {
    const digits = value.replace(/\D/g, '').slice(-9);
    const parsed = Number(digits);
    return Number.isNaN(parsed) ? Date.now() : parsed;
  }
}
