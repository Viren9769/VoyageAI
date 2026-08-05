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

export interface Trip {

    id: number;

    name: string;

    destinationCountry: string;

    destinationCity: string;

    coverImageUrl: string;

    startDateUtc: string;

    endDateUtc: string;

    totalDays: number;

    travelerCount: number;

    budgetCurrency: string;

    budgetAmount: number;

    status: 'Active' | 'Upcoming' | 'Completed' | 'Draft';

    lastUpdatedUtc: string;

}