namespace VoyageAI.API.DTOs.Itinerary
{
    /// <summary>
    /// DTO for returning an itinerary day summary (lightweight version).
    /// 
    /// This response is sent to the client for:
    /// - GET /api/trips/{tripId}/itinerary - Get all days for a trip (list view)
    /// 
    /// This is a lighter version of ItineraryDayResponse that omits audit trail details
    /// to reduce payload size when returning multiple days.
    /// 
    /// Convention:
    /// - DateTime fields are in ISO 8601 format in UTC
    /// - All monetary values use the trip's currency
    /// - Guids are in standard format (8-4-4-4-12 hex)
    /// </summary>
    public class ItinerarySummaryResponse
    {
        /// <summary>
        /// Unique identifier for this itinerary day.
        /// </summary>
        public Guid DayId { get; set; }

        /// <summary>
        /// The trip this day belongs to.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        /// The day number within the trip.
        /// Example: 1 = first day, 2 = second day, etc.
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// The date of this itinerary day.
        /// Format: ISO 8601 in UTC (e.g., "2025-06-15T00:00:00Z")
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title/heading for the day.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief summary of the day's activities and highlights.
        /// Useful for quick overview in list view.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Estimated budget for the day in the trip's currency.
        /// Useful for budget tracking and planning view.
        /// </summary>
        public decimal EstimatedBudget { get; set; }

        /// <summary>
        /// Actual budget spent on the day in the trip's currency.
        /// Useful for expense tracking.
        /// </summary>
        public decimal ActualBudget { get; set; }

        /// <summary>
        /// Weather summary or forecast for the day.
        /// Useful for quick planning reference.
        /// </summary>
        public string WeatherSummary { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the itinerary day was last modified.
        /// Allows client to display "Last updated on..." information.
        /// Format: ISO 8601 (e.g., "2025-01-06T14:35:00Z")
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indicates if this itinerary day is soft-deleted.
        /// If true, the day should not be displayed in the itinerary.
        /// Default: false.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
