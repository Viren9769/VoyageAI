import { TripDay } from './trip-day';

import { TravelTip } from './travel-tip';

export interface Itinerary {

    id: number;

    tripId: number;

    tripName: string;

    destination: string;

    startDate: string;

    endDate: string;

    duration: number;

    status: string;

    coverImage: string;

    days: TripDay[];

    travelTip: TravelTip;

}