export type TravelerType = 'Adult' | 'Child';

export interface Traveler {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  type: TravelerType;
  /** Age in years — only set for children */
  age?: number;
  tripCount: number;
  avatarUrl?: string;
  isPro?: boolean;
  assignedTripIds: string[];
}

export interface TravelerFormPayload {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  type: TravelerType;
  age?: number;
  assignedTripIds: string[];
}

export interface TravelerStats {
  totalTravelers: number;
  adults: number;
  children: number;
  upcomingTrips: number;
}

export interface TravelerFilter {
  search: string;
  tripId: string;
}
