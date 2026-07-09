namespace VoyageAI.API.DTOs.Itinerary
{
    /// <summary>
    /// DTO for updating an existing itinerary day.
    /// This request is sent by authenticated users to modify an itinerary day.
    /// 
    /// Security:
    /// - UserId is extracted from JWT token, not from request
    /// - User must own the trip containing this itinerary day
    /// 
    /// What Can Be Updated:
    /// - DayNumber: Can be changed, but must remain unique per trip
    /// - Date: Can be updated to any date within trip's date range
    /// - Title: Can be modified, required field
    /// - Summary, Notes, WeatherSummary: Can be modified or cleared
    /// - EstimatedBudget, ActualBudget: Can be updated with actual spending
    /// 
    /// What Cannot Be Updated:
    /// - DayId: Immutable, identifies the resource
    /// - TripId: Immutable, cannot move day to different trip
    /// - CreatedAt, CreatedBy: Immutable, set at creation
    /// 
    /// Validation:
    /// - DayNumber: Required, must be > 0, must be unique per trip (excluding current day)
    /// - Date: Required, must fall within trip's StartDate and EndDate
    /// - Title: Required, max 200 characters
    /// - Summary: Optional, max 1000 characters
    /// - Notes: Optional, max 3000 characters
    /// - Budgets: Optional, must be >= 0
    /// - WeatherSummary: Optional, max 500 characters
    /// </summary>
    public class UpdateItineraryDayRequest
    {
        /// <summary>
        /// The day number within the trip.
        /// Required. Must be > 0 and unique per trip (excluding this day's current number).
        /// Users can renumber days if they want to reorder the itinerary.
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// The date of this itinerary day.
        /// Required. Must fall within the trip's StartDate and EndDate (inclusive).
        /// Format: ISO 8601 (e.g., "2025-06-15T00:00:00Z")
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title/heading for the day.
        /// Required. Max length: 200 characters.
        /// Cannot be null or empty.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief summary of the day's activities and highlights.
        /// Optional. Max length: 1000 characters.
        /// Can be cleared by sending empty string.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Detailed notes for the day.
        /// Optional. Max length: 3000 characters.
        /// Can be cleared by sending empty string.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Estimated budget for the day.
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// </summary>
        public decimal EstimatedBudget { get; set; } = 0;

        /// <summary>
        /// Actual budget spent on the day.
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// Typically updated as the trip progresses and actual expenses are known.
        /// </summary>
        public decimal ActualBudget { get; set; } = 0;

        /// <summary>
        /// Weather summary or forecast for the day.
        /// Optional. Max length: 500 characters.
        /// Can be cleared by sending empty string or null.
        /// </summary>
        public string? WeatherSummary { get; set; }
    }
}
