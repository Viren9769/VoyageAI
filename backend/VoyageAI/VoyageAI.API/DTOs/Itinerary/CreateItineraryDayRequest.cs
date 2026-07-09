namespace VoyageAI.API.DTOs.Itinerary
{
    /// <summary>
    /// DTO for creating a new itinerary day within a trip.
    /// This request is sent by authenticated users to add a day to their trip's itinerary.
    /// 
    /// Security:
    /// - UserId is extracted from JWT token, not from request
    /// - User must own the trip to add itinerary days
    /// 
    /// Validation:
    /// - DayNumber: Required, must be > 0, must be unique per trip
    /// - Date: Required, must fall within trip's StartDate and EndDate
    /// - Title: Required, max 200 characters
    /// - Summary: Optional, max 1000 characters
    /// - Notes: Optional, max 3000 characters
    /// - Budgets: Optional, must be >= 0
    /// - WeatherSummary: Optional, max 500 characters
    /// </summary>
    public class CreateItineraryDayRequest
    {
        /// <summary>
        /// The day number within the trip (e.g., 1 for first day, 2 for second day).
        /// Required. Must be > 0 and unique per trip.
        /// If user provides day numbers out of sequence, they are accepted as-is
        /// (e.g., a trip can have day 1, day 2, day 5 without day 3 and 4).
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// The date of this itinerary day.
        /// Required. Must fall within the trip's StartDate and EndDate (inclusive).
        /// Format: ISO 8601 (e.g., "2025-06-15T00:00:00Z")
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title/heading for the day (e.g., "Tour of Eiffel Tower and Louvre Museum").
        /// Required. Max length: 200 characters.
        /// Cannot be null or empty.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief summary of the day's activities and highlights.
        /// Optional. Max length: 1000 characters.
        /// Null/empty is acceptable.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Detailed notes for the day (accommodations, special arrangements, confirmations, etc.).
        /// Optional. Max length: 3000 characters.
        /// Can contain booking reference numbers, notes about reservations, etc.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Estimated budget for the day in the trip's currency.
        /// Optional. Default: 0.
        /// Must be >= 0. No upper limit enforced at validation level.
        /// </summary>
        public decimal EstimatedBudget { get; set; } = 0;

        /// <summary>
        /// Actual budget spent on the day (typically updated after the day passes).
        /// Optional. Default: 0.
        /// Must be >= 0. No upper limit enforced at validation level.
        /// Can be provided at creation time (for past trips) or updated later.
        /// </summary>
        public decimal ActualBudget { get; set; } = 0;

        /// <summary>
        /// Weather summary or forecast for the day.
        /// Example: "Sunny, 25°C, Low chance of rain", "Rainy, 15°C, Bring an umbrella"
        /// Optional. Max length: 500 characters.
        /// Null/empty is acceptable.
        /// </summary>
        public string? WeatherSummary { get; set; }
    }
}
