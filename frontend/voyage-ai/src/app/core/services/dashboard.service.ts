import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import { DashboardData } from '../../models/dashboard';
import { ApiResponse } from '../../models/api-response';

interface DashboardResponse {
  welcome: {
    greeting: string;
    userName: string;
    subtitle: string;
  };
  stats: Array<{ title: string; value: string | number; icon: string; change: number; trend: 'up' | 'down'; color: string }>;
  aiPlanner: { title: string; subtitle: string; placeholder: string; buttonText: string };
  upcomingTrip: { title: string; destination: string; image: string; startDate: string; endDate: string; daysLeft: number; progress: number };
  weather: { city: string; country: string; temperature: number; condition: string; icon: string; humidity: number; windSpeed: number; feelsLike: number };
  reminders: Array<{ title: string; date: string; icon: string; color: string; timeLeft: string }>;
  travelMap: { title: string; image: string; destinations: Array<{ name: string; top: string; left: string }> };
  expenseOverview: { total: string; period: string; change: number; categories: Array<{ name: string; amount: string; percent: number; color: string }> };
  recentTrips: Array<{ title: string; country: string; image: string; startDate: string; endDate: string; status: string }>;
  travelerProfile: { name: string; avatar: string; level: number; voyagePoints: number };
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private readonly http = inject(HttpClient);
  private readonly apiUrl = ApiConfig.baseUrl + '/dashboard';

  getDashboard(): Observable<DashboardData> {
    console.log('DashboardService: Calling GET /api/dashboard');
    return this.http
      .get<ApiResponse<DashboardData>>(this.apiUrl)
      .pipe(
        tap(response => {
          console.log('DashboardService: Raw API response received:', response);
        }),
        map(response => {
          console.log('DashboardService: Mapping response.data:', response.data);
          return response.data;
        }),
        tap(data => {
          console.log('DashboardService: After mapping, returning data:', data);
        })
      );
  }

}
