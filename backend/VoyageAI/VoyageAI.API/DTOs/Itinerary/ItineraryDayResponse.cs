namespace VoyageAI.API.DTOs.Itinerary
{
    /// <summary>
    /// DTO for returning a detailed itinerary day response.
    /// 
    /// This response is sent to the client for:
    /// - GET /api/trips/{tripId}/itinerary/{dayId} - Get single day details
    /// - POST /api/trips/{tripId}/itinerary - Create day returns this
    /// - PUT /api/trips/{tripId}/itinerary/{dayId} - Update day returns this
    /// 
    /// Includes all fields including audit trail for transparency.
    /// 
    /// Convention:
    /// - DateTime fields are in ISO 8601 format in UTC
    /// - All monetary values use the trip's currency
    /// - Guids are in standard format (8-4-4-4-12 hex)
    /// </summary>
    public class ItineraryDayResponse
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
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Detailed notes for the day.
        /// Can include booking references, accommodation details, special instructions, etc.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Estimated budget for the day in the trip's currency.
        /// </summary>
        public decimal EstimatedBudget { get; set; }

        /// <summary>
        /// Actual budget spent on the day in the trip's currency.
        /// </summary>
        public decimal ActualBudget { get; set; }

        /// <summary>
        /// Weather summary or forecast for the day.
        /// </summary>
        public string WeatherSummary { get; set; } = string.Empty;

        // ============================================================
        // AUDIT TRAIL
        // ============================================================

        /// <summary>
        /// UTC timestamp when the itinerary day was created.
        /// Format: ISO 8601 (e.g., "2025-01-06T14:30:00Z")
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the itinerary day was last modified.
        /// Format: ISO 8601 (e.g., "2025-01-06T14:35:00Z")
        /// Will be same as CreatedAt if never modified.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// UserId of the user who created this itinerary day.
        /// Useful for audit purposes and understanding who created the entry.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// UserId of the user who last modified this itinerary day.
        /// Useful for audit purposes and understanding who made the latest changes.
        /// Will be same as CreatedBy if never modified.
        /// </summary>
        public Guid LastModifiedBy { get; set; }

        /// <summary>
        /// Indicates if this itinerary day is soft-deleted.
        /// If true, the day should not be displayed in the itinerary.
        /// Default: false.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// UTC timestamp when the itinerary day was deleted (soft delete).
        /// Null if not deleted.
        /// Format: ISO 8601 (e.g., "2025-01-06T15:00:00Z")
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
