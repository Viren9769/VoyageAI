export interface TripData {

    id: number;

    title: string;

    destination: string;

    image: string;

    startDate: string;

    endDate: string;

    days: number;

    travelers: number;

    progress: number;

    status: 'Active' | 'Upcoming' | 'Completed' | 'Draft';

}