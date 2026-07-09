namespace VoyageAI.API.Models.Enums
{
    /// <summary>
    /// Enumeration of activity categories.
    /// 
    /// These categories help organize and filter activities within an itinerary day.
    /// Users can use these to classify their planned activities.
    /// 
    /// Categories:
    /// - Sightseeing: Tourist attractions, landmarks, viewpoints
    /// - Restaurant: Dining establishments (breakfast, lunch, dinner)
    /// - Museum: Museums, galleries, exhibitions
    /// - Shopping: Shopping malls, markets, stores
    /// - Transportation: Travel methods (flights, trains, buses, transfers)
    /// - Adventure: Adventure sports, adventure activities
    /// - Entertainment: Shows, concerts, nightlife, movies
    /// - Hotel: Hotel check-in, accommodations
    /// - Flight: Flight bookings and travel
    /// - Other: Miscellaneous activities not fitting other categories
    /// 
    /// Usage:
    /// When creating or updating an activity, the category must be one of these values.
    /// Categories are used for:
    /// - Activity filtering and search
    /// - Cost analysis by category
    /// - Trip planning and optimization
    /// - Future AI-powered itinerary suggestions
    /// 
    /// Examples:
    /// - "Visit Eiffel Tower" -> Sightseeing
    /// - "Dinner at local bistro" -> Restaurant
    /// - "Flight to Rome" -> Flight
    /// - "Rock climbing" -> Adventure
    /// </summary>
    public enum ActivityCategory
    {
        /// <summary>
        /// Tourist attractions, landmarks, viewpoints, scenic locations.
        /// Value: 0
        /// </summary>
        Sightseeing = 0,

        /// <summary>
        /// Dining establishments: breakfast, lunch, dinner, cafes.
        /// Value: 1
        /// </summary>
        Restaurant = 1,

        /// <summary>
        /// Museums, art galleries, exhibitions, cultural centers.
        /// Value: 2
        /// </summary>
        Museum = 2,

        /// <summary>
        /// Shopping malls, markets, stores, souvenirs shopping.
        /// Value: 3
        /// </summary>
        Shopping = 3,

        /// <summary>
        /// Transportation: flights, trains, buses, car rentals, transfers.
        /// Value: 4
        /// </summary>
        Transportation = 4,

        /// <summary>
        /// Adventure activities: hiking, rock climbing, water sports, extreme sports.
        /// Value: 5
        /// </summary>
        Adventure = 5,

        /// <summary>
        /// Entertainment: shows, concerts, theater, nightlife, cinema, amusement parks.
        /// Value: 6
        /// </summary>
        Entertainment = 6,

        /// <summary>
        /// Hotel accommodations, check-in activities, resort amenities.
        /// Value: 7
        /// </summary>
        Hotel = 7,

        /// <summary>
        /// Flight bookings and air travel.
        /// Value: 8
        /// </summary>
        Flight = 8,

        /// <summary>
        /// Miscellaneous activities not fitting other categories.
        /// Value: 9
        /// </summary>
        Other = 9
    }
}
