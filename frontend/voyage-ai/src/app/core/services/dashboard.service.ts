import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';

import { DashboardData } from '../../models/dashboard';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor() { }

  getDashboard(): Observable<DashboardData> {

    const user = JSON.parse(
      localStorage.getItem('voyage_user') ?? '{}'
    );

    const firstName = user.firstName ?? 'Traveler';

    const dashboard: DashboardData = {

      /* ==========================================
      WELCOME
      ========================================== */

      welcome: {

        greeting: 'Welcome back,',

        userName: `${firstName} 👋`,

        subtitle: 'Ready for your next adventure?'

      },

      /* ==========================================
      STATS
      ========================================== */

      stats: [

        {
          title: 'Total Trips',
          value: 18,
          icon: 'luggage',
          change: 24,
          trend: 'up',
          color: '#8B5CF6'
        },

        {
          title: 'Countries Visited',
          value: 9,
          icon: 'public',
          change: 12,
          trend: 'up',
          color: '#3B82F6'
        },

        {
          title: 'Total Budget',
          value: '$14,230',
          icon: 'account_balance_wallet',
          change: 8,
          trend: 'down',
          color: '#F59E0B'
        },

        {
          title: 'Total Travelers',
          value: 27,
          icon: 'groups',
          change: 16,
          trend: 'up',
          color: '#10B981'
        }

      ],

      /* ==========================================
      AI PLANNER
      ========================================== */

      aiPlanner: {

        title: 'AI Trip Planner',

        subtitle:
          'Describe your dream trip and let VoyageAI build the perfect itinerary for you.',

        placeholder:
          'Example: Plan a 7-day trip to Switzerland with a budget of $3000',

        buttonText: 'Generate Trip'

      }

    };

    return of(dashboard);

  }

}