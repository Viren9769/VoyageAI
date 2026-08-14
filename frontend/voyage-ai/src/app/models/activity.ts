export interface Activity {

    id: number;

    tripId: string;

    dayNumber: number;

    title: string;

    description: string;

    locationName: string;

    startTime: string;

    endTime: string;

    // Backward-compatible field used by legacy templates while migration to start/end time is in progress.
    time: string;

    icon: string;

    status: 'Confirmed' | 'Pending' | 'Suggested';

    category:
        | 'Flight'
        | 'Hotel'
        | 'Food'
        | 'Transport'
        | 'Activity'
        | 'Shopping'
        | 'Other';

    estimatedCost: number;

    currency: string;

    confirmationCode?: string;

}