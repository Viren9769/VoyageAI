export interface CreateTrip {

    tripName: string;

    destinationCountry: string;

    destinationCity: string;

    startDate: string;

    endDate: string;

    budget: number;

    currency: string;

    travelStyle: string;

    description: string;

    coverImageUrl: string;

    status: string;

}

export type TripStatus =
    | 'Draft'
    | 'Upcoming'
    | 'Active'
    | 'Completed';

export type TravelStyle =
    | 'Luxury'
    | 'Adventure'
    | 'Family'
    | 'Romantic'
    | 'Solo'
    | 'Business'
    | 'Road Trip'
    | 'Backpacking'
    | 'Cruise'
    | 'Camping';