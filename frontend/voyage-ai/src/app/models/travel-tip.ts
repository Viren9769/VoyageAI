export interface TravelTip {

    id: number;

    tripId: number;

    dayNumber: number;

    title: string;

    description: string;

    imageUrl: string;

    temperatureCelsius: number;

    icon: string;

    recommendedWindow: string;

    source: string;

    aiConfidence: number;

}