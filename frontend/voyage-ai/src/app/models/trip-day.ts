export interface TripDay {

    id: number;

    tripId: number;

    dayNumber: number;

    title: string;

    date: string;

    theme: string;

    isTransitDay: boolean;

    notes?: string;

}