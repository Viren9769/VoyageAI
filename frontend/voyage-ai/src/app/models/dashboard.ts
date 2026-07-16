/* ============================================================
DASHBOARD
============================================================ */

export interface DashboardData {

  welcome: WelcomeSection;

  stats: DashboardStat[];
  
  aiPlanner: AiPlanner;

  upcomingTrip: UpcomingTrip;

  weather: WeatherCard;

  reminders: Reminder[];

  travelMap: TravelMap;

  expenseOverview: ExpenseOverview;

  recentTrips: RecentTrip[];

}

/* ============================================================
WELCOME
============================================================ */

export interface WelcomeSection {

  greeting: string;

  userName: string;

  subtitle: string;

}

/* ============================================================
STATS CARD
============================================================ */

export interface DashboardStat {

  title: string;

  value: number | string;

  icon: string;

  change: number;

  trend: 'up' | 'down';

  color: string;

}
/* ============================================================
AI PLANNER
============================================================ */

export interface AiPlanner {

  title: string;

  subtitle: string;

  placeholder: string;

  buttonText: string;

}
/* ============================================================
UPCOMING TRIP
============================================================ */

export interface UpcomingTrip {

  title: string;

  destination: string;

  image: string;

  startDate: string;

  endDate: string;

  daysLeft: number;

  progress: number;

}

/* ============================================================
Weather Card
============================================================ */
export interface WeatherCard {

  city: string;

  country: string;

  temperature: number;

  condition: string;

  icon: string;

  humidity: number;

  windSpeed: number;

  feelsLike: number;

}


/* ============================================================
UPCOMING REMINDERS
============================================================ */

export interface Reminder {

  title: string;

  date: string;

  icon: string;

  color: string;

  timeLeft: string;

}
/* ============================================================
Travel Map
============================================================ */

export interface TravelMap {

  title: string;

  image: string;

  destinations: Destination[];

}

export interface Destination {

  name: string;

  top: string;

  left: string;

}

/* ============================================================
EXPENSE OVERVIEW
============================================================ */

export interface ExpenseOverview {

  total: string;

  period: string;

  change: number;

  categories: ExpenseCategory[];

}

export interface ExpenseCategory {

  name: string;

  amount: string;

  percent: number;

  color: string;

}

/* ============================================================
RECENT TRIPS
============================================================ */

export interface RecentTrip {

  title: string;

  country: string;

  image: string;

  startDate: string;

  endDate: string;

  status: 'Completed' | 'Upcoming' | 'Planning';

}