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

      },

      upcomingTrip: {

  title: 'Switzerland Escape',

  destination: 'Switzerland',

  image: 'images/switzerland.jpg',

  startDate: 'Jun 18, 2026',

  endDate: 'Jun 26, 2026',

  daysLeft: 8,

  progress: 75

},

weather: {

  city: 'Zurich',

  country: 'Switzerland',

  temperature: 22,

  condition: 'Sunny',

  icon: 'wb_sunny',

  humidity: 35,

  windSpeed: 12,

  feelsLike: 22

},

reminders: [

  {
    title: 'Flight to Zurich',
    date: 'Jun 18, 2026 • 10:30 PM',
    icon: 'flight_takeoff',
    color: '#2563EB',
    timeLeft: 'In 8 days'
  },

  {
    title: 'Check-in Opens',
    date: 'Jun 18, 2026 • 3:00 PM',
    icon: 'event_available',
    color: '#8B5CF6',
    timeLeft: 'In 8 days'
  },

  {
    title: 'Jungfraujoch Day Trip',
    date: 'Jun 20, 2026 • 9:00 AM',
    icon: 'landscape',
    color: '#F97316',
    timeLeft: 'In 10 days'
  }

],

travelMap: {

  title: 'My Travel Map',

  image: 'images/world.svg',

  destinations: [

    {
      name: 'USA',
      top: '38%',
      left: '18%'
    },

    {
      name: 'Switzerland',
      top: '34%',
      left: '49%'
    },

    {
      name: 'Japan',
      top: '38%',
      left: '77%'
    },

    {
      name: 'Australia',
      top: '73%',
      left: '82%'
    }

  ]

},
expenseOverview: {

  total: '$14,230',

  period: 'This Year',

  change: -8,

  categories: [

    {
      name: 'Flights',
      amount: '$5,320',
      percent: 37,
      color: '#6D5DFB'
    },

    {
      name: 'Hotels',
      amount: '$4,230',
      percent: 30,
      color: '#8B5CF6'
    },

    {
      name: 'Food',
      amount: '$2,450',
      percent: 17,
      color: '#4FD1C5'
    },

    {
      name: 'Activities',
      amount: '$1,520',
      percent: 11,
      color: '#FBBF24'
    },

    {
      name: 'Transport',
      amount: '$710',
      percent: 5,
      color: '#F87171'
    }

  ]

},
recentTrips: [

  {
    title: 'Swiss Adventure',
    country: 'Switzerland',
    image: 'images/switzerland.jpg',
    startDate: 'Jun 18, 2026',
    endDate: 'Jun 26, 2026',
    status: 'Completed'
  },

  {
    title: 'Tokyo Explorer',
    country: 'Japan',
    image: 'images/japan.jpeg',
    startDate: 'Sep 10, 2026',
    endDate: 'Sep 18, 2026',
    status: 'Upcoming'
  },

  {
    title: 'Sydney Escape',
    country: 'Australia',
    image: 'images/australia.jpeg',
    startDate: 'Nov 5, 2026',
    endDate: 'Nov 14, 2026',
    status: 'Planning'
  }

],
travelerProfile: {

  name: `${user.firstName ?? ''} ${user.lastName ?? ''}`.trim(),

  avatar:
    user.profilePicture ||
    user.avatar ||
    'images/avatar.png',

  level: user.level ?? 12,

  voyagePoints: user.voyagePoints ?? 2450

},

    };

    return of(dashboard);

  }

}