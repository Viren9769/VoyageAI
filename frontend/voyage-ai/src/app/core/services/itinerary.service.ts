import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { Trip } from '../../models/trip';
import { TripDay } from '../../models/trip-day';
import { Activity } from '../../models/activity';
import { TravelTip } from '../../models/travel-tip';

@Injectable({
  providedIn: 'root'
})
export class ItineraryService {

  constructor() { }

  private trips: Trip[] = [
    {
      id: 1,
      name: 'Switzerland Escape',
      destinationCountry: 'Switzerland',
      destinationCity: 'Zurich',
      coverImageUrl: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=600',
      startDateUtc: '2026-06-18T00:00:00Z',
      endDateUtc: '2026-06-26T00:00:00Z',
      totalDays: 8,
      travelerCount: 4,
      budgetCurrency: 'USD',
      budgetAmount: 5400,
      status: 'Active',
      lastUpdatedUtc: '2026-06-10T14:35:00Z'
    },
    {
      id: 2,
      name: 'Tokyo Explorer',
      destinationCountry: 'Japan',
      destinationCity: 'Tokyo',
      coverImageUrl: 'https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=600',
      startDateUtc: '2026-07-20T00:00:00Z',
      endDateUtc: '2026-07-27T00:00:00Z',
      totalDays: 8,
      travelerCount: 2,
      budgetCurrency: 'USD',
      budgetAmount: 6200,
      status: 'Upcoming',
      lastUpdatedUtc: '2026-07-02T09:20:00Z'
    },
    {
      id: 3,
      name: 'Bali Adventure',
      destinationCountry: 'Indonesia',
      destinationCity: 'Ubud',
      coverImageUrl: 'https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=600',
      startDateUtc: '2026-04-05T00:00:00Z',
      endDateUtc: '2026-04-12T00:00:00Z',
      totalDays: 8,
      travelerCount: 2,
      budgetCurrency: 'USD',
      budgetAmount: 3100,
      status: 'Completed',
      lastUpdatedUtc: '2026-04-12T20:10:00Z'
    },
    {
      id: 4,
      name: 'Paris Getaway',
      destinationCountry: 'France',
      destinationCity: 'Paris',
      coverImageUrl: 'https://images.unsplash.com/photo-1431274172761-fca41d930114?w=600',
      startDateUtc: '2026-05-15T00:00:00Z',
      endDateUtc: '2026-05-22T00:00:00Z',
      totalDays: 8,
      travelerCount: 2,
      budgetCurrency: 'USD',
      budgetAmount: 4700,
      status: 'Completed',
      lastUpdatedUtc: '2026-05-22T18:00:00Z'
    },
    {
      id: 5,
      name: 'Iceland Road Trip',
      destinationCountry: 'Iceland',
      destinationCity: 'Reykjavik',
      coverImageUrl: 'https://images.unsplash.com/photo-1476610182048-b716b8518aae?w=600',
      startDateUtc: '2026-09-10T00:00:00Z',
      endDateUtc: '2026-09-17T00:00:00Z',
      totalDays: 8,
      travelerCount: 4,
      budgetCurrency: 'USD',
      budgetAmount: 7800,
      status: 'Draft',
      lastUpdatedUtc: '2026-08-03T11:00:00Z'
    },
    {
      id: 6,
      name: 'Dubai Luxury',
      destinationCountry: 'UAE',
      destinationCity: 'Dubai',
      coverImageUrl: 'https://images.unsplash.com/photo-1512453979798-5ea266f8880c?w=600',
      startDateUtc: '2026-10-05T00:00:00Z',
      endDateUtc: '2026-10-12T00:00:00Z',
      totalDays: 8,
      travelerCount: 3,
      budgetCurrency: 'USD',
      budgetAmount: 9200,
      status: 'Upcoming',
      lastUpdatedUtc: '2026-08-01T10:00:00Z'
    },
    {
      id: 7,
      name: 'New York City Break',
      destinationCountry: 'USA',
      destinationCity: 'New York',
      coverImageUrl: 'https://images.unsplash.com/photo-1499092346589-b9b6be3e94b2?w=600',
      startDateUtc: '2026-11-18T00:00:00Z',
      endDateUtc: '2026-11-23T00:00:00Z',
      totalDays: 6,
      travelerCount: 2,
      budgetCurrency: 'USD',
      budgetAmount: 5100,
      status: 'Draft',
      lastUpdatedUtc: '2026-08-04T16:10:00Z'
    },
    {
      id: 8,
      name: 'Santorini Getaway',
      destinationCountry: 'Greece',
      destinationCity: 'Santorini',
      coverImageUrl: 'https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=600',
      startDateUtc: '2026-12-10T00:00:00Z',
      endDateUtc: '2026-12-16T00:00:00Z',
      totalDays: 7,
      travelerCount: 2,
      budgetCurrency: 'USD',
      budgetAmount: 6500,
      status: 'Completed',
      lastUpdatedUtc: '2026-01-05T09:10:00Z'
    }
  ];

  private daysByTrip: Record<number, TripDay[]> = {
    1: [
      { id: 101, tripId: 1, dayNumber: 1, title: 'Arrival in Zurich', date: '18 Jun', theme: 'Arrival', isTransitDay: true },
      { id: 102, tripId: 1, dayNumber: 2, title: 'Zurich Old Town', date: '19 Jun', theme: 'Culture', isTransitDay: false },
      { id: 103, tripId: 1, dayNumber: 3, title: 'Lucerne and Lake Cruise', date: '20 Jun', theme: 'Scenic', isTransitDay: false },
      { id: 104, tripId: 1, dayNumber: 4, title: 'Interlaken Adventure', date: '21 Jun', theme: 'Adventure', isTransitDay: false }
    ],
    2: [
      { id: 201, tripId: 2, dayNumber: 1, title: 'Shinjuku Arrival', date: '20 Jul', theme: 'Arrival', isTransitDay: true },
      { id: 202, tripId: 2, dayNumber: 2, title: 'Asakusa and Senso-ji', date: '21 Jul', theme: 'Culture', isTransitDay: false },
      { id: 203, tripId: 2, dayNumber: 3, title: 'Akihabara and TeamLab', date: '22 Jul', theme: 'Tech', isTransitDay: false }
    ],
    3: [
      { id: 301, tripId: 3, dayNumber: 1, title: 'Ubud Check-In', date: '05 Apr', theme: 'Arrival', isTransitDay: true },
      { id: 302, tripId: 3, dayNumber: 2, title: 'Rice Terrace Tour', date: '06 Apr', theme: 'Nature', isTransitDay: false },
      { id: 303, tripId: 3, dayNumber: 3, title: 'Temple Trail', date: '07 Apr', theme: 'Culture', isTransitDay: false }
    ]
  };

  private activitiesByTripDay: Record<string, Activity[]> = {
    '1-1': [
      { id: 1001, tripId: 1, dayNumber: 1, title: 'Flight Arrival', description: 'Land at Zurich Airport and clear immigration.', locationName: 'Zurich Airport', startTime: '08:30 AM', endTime: '09:45 AM', time: '08:30 AM', icon: 'flight_land', status: 'Confirmed', category: 'Flight', estimatedCost: 0, currency: 'USD', confirmationCode: 'LX-4492' },
      { id: 1002, tripId: 1, dayNumber: 1, title: 'Hotel Check-in', description: 'Early check-in at the city center hotel.', locationName: 'Hilton Zurich Airport', startTime: '11:00 AM', endTime: '11:45 AM', time: '11:00 AM', icon: 'hotel', status: 'Confirmed', category: 'Hotel', estimatedCost: 320, currency: 'USD' },
      { id: 1003, tripId: 1, dayNumber: 1, title: 'Evening Walk', description: 'Leisure walk around Bahnhofstrasse.', locationName: 'Bahnhofstrasse', startTime: '05:00 PM', endTime: '06:30 PM', time: '05:00 PM', icon: 'directions_walk', status: 'Suggested', category: 'Activity', estimatedCost: 0, currency: 'USD' }
    ],
    '1-2': [
      { id: 1004, tripId: 1, dayNumber: 2, title: 'Old Town Guided Tour', description: 'Local guided walking tour through historic districts.', locationName: 'Zurich Old Town', startTime: '09:00 AM', endTime: '11:30 AM', time: '09:00 AM', icon: 'explore', status: 'Confirmed', category: 'Activity', estimatedCost: 85, currency: 'USD' },
      { id: 1005, tripId: 1, dayNumber: 2, title: 'Swiss Lunch', description: 'Traditional fondue lunch.', locationName: 'Restaurant Zeughauskeller', startTime: '01:00 PM', endTime: '02:00 PM', time: '01:00 PM', icon: 'restaurant', status: 'Confirmed', category: 'Food', estimatedCost: 55, currency: 'USD' }
    ],
    '2-1': [
      { id: 2001, tripId: 2, dayNumber: 1, title: 'Airport Limousine Bus', description: 'Transfer from Haneda to hotel.', locationName: 'Haneda Airport', startTime: '10:30 AM', endTime: '11:30 AM', time: '10:30 AM', icon: 'directions_bus', status: 'Confirmed', category: 'Transport', estimatedCost: 25, currency: 'USD' },
      { id: 2002, tripId: 2, dayNumber: 1, title: 'Shinjuku Dinner', description: 'Welcome dinner in Omoide Yokocho.', locationName: 'Shinjuku', startTime: '07:00 PM', endTime: '08:30 PM', time: '07:00 PM', icon: 'dinner_dining', status: 'Pending', category: 'Food', estimatedCost: 70, currency: 'USD' }
    ],
    '3-1': [
      { id: 3001, tripId: 3, dayNumber: 1, title: 'Villa Transfer', description: 'Private transfer to Ubud villa.', locationName: 'Ngurah Rai Airport', startTime: '09:45 AM', endTime: '11:15 AM', time: '09:45 AM', icon: 'airport_shuttle', status: 'Confirmed', category: 'Transport', estimatedCost: 35, currency: 'USD' }
    ]
  };

  private travelTipByTripDay: Record<string, TravelTip> = {
    '1-1': {
      id: 9001,
      tripId: 1,
      dayNumber: 1,
      title: 'Arrival Day Recommendation',
      description: 'Keep the first evening light to adapt to local time. A short walk and early dinner can reduce jet lag before day two.',
      imageUrl: 'https://images.unsplash.com/photo-1516483638261-f4dbaf036963?w=800',
      temperatureCelsius: 18,
      icon: 'auto_awesome',
      recommendedWindow: '5:00 PM - 7:00 PM',
      source: 'Voyage AI Assistant',
      aiConfidence: 0.93
    },
    '1-2': {
      id: 9002,
      tripId: 1,
      dayNumber: 2,
      title: 'City Walk Optimization',
      description: 'Visit viewpoints before noon to avoid larger tour groups and keep museum visits for late afternoon.',
      imageUrl: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=800',
      temperatureCelsius: 20,
      icon: 'auto_awesome',
      recommendedWindow: '8:30 AM - 11:30 AM',
      source: 'Voyage AI Assistant',
      aiConfidence: 0.91
    }
  };

  // Keep method signatures API-ready so switching to HttpClient only changes this service.
  getTrips(): Observable<Trip[]> {
    return of(this.trips);
  }

  getTripDays(tripId: number): Observable<TripDay[]> {
    const staticDays = this.daysByTrip[tripId];

    if (staticDays) {
      return of(staticDays);
    }

    return of(this.buildFallbackTripDays(tripId));
  }

  getActivities(tripId: number, dayNumber: number): Observable<Activity[]> {
    const activities = this.activitiesByTripDay[this.getTripDayKey(tripId, dayNumber)] ?? this.buildFallbackActivities(tripId, dayNumber);
    return of(activities);
  }

  getTravelTip(tripId: number, dayNumber: number): Observable<TravelTip> {
    const defaultTip: TravelTip = {
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

    return of(this.travelTipByTripDay[this.getTripDayKey(tripId, dayNumber)] ?? defaultTip);
  }

  private getTripDayKey(tripId: number, dayNumber: number): string {
    return `${tripId}-${dayNumber}`;
  }

  private buildFallbackTripDays(tripId: number): TripDay[] {
    const trip = this.trips.find(x => x.id === tripId);

    if (!trip) {
      return [];
    }

    const startDate = new Date(trip.startDateUtc);

    return Array.from({ length: trip.totalDays }, (_, index) => {
      const dayNumber = index + 1;
      const date = new Date(startDate);
      date.setDate(startDate.getDate() + index);

      return {
        id: tripId * 100 + dayNumber,
        tripId,
        dayNumber,
        title: dayNumber === 1 ? `Arrival in ${trip.destinationCity}` : `Explore ${trip.destinationCity}`,
        date: this.formatDayMonth(date),
        theme: dayNumber % 2 === 0 ? 'City Highlights' : 'Local Discovery',
        isTransitDay: dayNumber === 1
      };
    });
  }

  private buildFallbackActivities(tripId: number, dayNumber: number): Activity[] {
    return [
      {
        id: tripId * 1000 + dayNumber * 10 + 1,
        tripId,
        dayNumber,
        title: 'Morning Exploration',
        description: 'Start the day with a flexible exploration slot and local coffee stop.',
        locationName: 'City Center',
        startTime: '09:00 AM',
        endTime: '10:30 AM',
        time: '09:00 AM',
        icon: 'explore',
        status: 'Suggested',
        category: 'Activity',
        estimatedCost: 20,
        currency: 'USD'
      },
      {
        id: tripId * 1000 + dayNumber * 10 + 2,
        tripId,
        dayNumber,
        title: 'Local Dining',
        description: 'Lunch reservation at a highly rated local restaurant.',
        locationName: 'Old Town District',
        startTime: '01:00 PM',
        endTime: '02:00 PM',
        time: '01:00 PM',
        icon: 'restaurant',
        status: 'Suggested',
        category: 'Food',
        estimatedCost: 45,
        currency: 'USD'
      }
    ];
  }

  private formatDayMonth(date: Date): string {
    return new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short' }).format(date);
  }
}