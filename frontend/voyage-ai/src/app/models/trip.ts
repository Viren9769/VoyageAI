export interface TripData {

    id: number;

    backendId?: string;

    title: string;

    destination: string;

    image: string;

    startDate: string;

    endDate: string;

    days: number;

    travelers: number;

    progress: number;

    budget?: number;

    currency?: string;

    status: 'Active' | 'Upcoming' | 'Completed' | 'Draft';

}

export interface Trip {

    id: string;

    backendId?: string;

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