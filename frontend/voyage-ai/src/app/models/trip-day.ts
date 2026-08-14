export interface TripDay {

    id: number;

    tripId: string;

    dayNumber: number;

    title: string;

    date: string;

    theme: string;

    isTransitDay: boolean;

    notes?: string;

}