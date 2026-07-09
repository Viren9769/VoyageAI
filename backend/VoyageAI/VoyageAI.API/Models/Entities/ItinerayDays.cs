namespace VoyageAI.API.Models.Entities
{
    /// <summary>
    /// ItineraryDay entity representing a single day within a trip's itinerary.
    /// 
    /// A Trip can have multiple ItineraryDays, one for each day of the trip.
    /// Each ItineraryDay can contain multiple Activities.
    /// 
    /// Soft Delete Pattern:
    /// - ItineraryDays are never physically deleted
    /// - Instead, IsDeleted is set to true and DeletedAt records the time
    /// - Queries must filter out deleted days
    /// 
    /// Audit Fields:
    /// - CreatedAt: When the itinerary day was created
    /// - UpdatedAt: When the itinerary day was last modified
    /// - CreatedBy: UserId of who created the itinerary day
    /// - LastModifiedBy: UserId of who last modified the itinerary day
    /// </summary>
    public class ItineraryDay
    {
        /// <summary>
        /// Unique identifier for the itinerary day.
        /// </summary>
        public Guid DayId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Foreign key to the Trip this day belongs to.
        /// CRITICAL: Every itinerary day must belong to exactly ONE trip.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        /// The day number within the trip (e.g., 1 for first day, 2 for second day).
        /// Must be unique per Trip.
        /// Required: must be greater than 0.
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// The date of this itinerary day.
        /// Required: must fall within the Trip's StartDate and EndDate.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Title/heading for the day (e.g., "Tour of Eiffel Tower and Louvre Museum").
        /// Required. Max length: 200 characters.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief summary of the day's activities and highlights.
        /// Optional. Max length: 1000 characters.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Detailed notes for the day (accommodations, special arrangements, etc.).
        /// Optional. Max length: 3000 characters.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Estimated budget for the day in the trip's currency.
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// </summary>
        public decimal EstimatedBudget { get; set; } = 0;

        /// <summary>
        /// Actual budget spent on the day (updated after the day passes).
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// </summary>
        public decimal ActualBudget { get; set; } = 0;

        /// <summary>
        /// Weather summary or forecast for the day (e.g., "Sunny, 25°C, Low chance of rain").
        /// Optional. Max length: 500 characters.
        /// </summary>
        public string WeatherSummary { get; set; } = string.Empty;

        // ============================================================
        // SOFT DELETE & AUDIT FIELDS
        // ============================================================

        /// <summary>
        /// Indicates if this itinerary day record is deleted.
        /// Soft delete pattern: IsDeleted = true instead of physical deletion.
        /// Default: false.
        /// 
        /// Index: (TripId, IsDeleted) for efficient querying of active days.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// UTC timestamp when the itinerary day was deleted.
        /// Null if not deleted.
        /// Only set when IsDeleted becomes true.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the itinerary day record was created.
        /// Set automatically by the service layer.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UTC timestamp when the itinerary day record was last modified.
        /// Updated every time any field changes.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UserId of the user who created this itinerary day record.
        /// Extracted from JWT token at creation time.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// UserId of the user who last modified this itinerary day record.
        /// Updated every time the itinerary day is modified.
        /// </summary>
        public Guid LastModifiedBy { get; set; }

        // ============================================================
        // NAVIGATION PROPERTIES
        // ============================================================

        /// <summary>
        /// Navigation property to the Trip this day belongs to.
        /// CRITICAL: An itinerary day can only belong to ONE trip.
        /// Always populated by EF Core queries.
        /// </summary>
        public Trip Trip { get; set; }

        /// <summary>
        /// Collection of activities planned for this day.
        /// Can be empty if no activities are planned yet.
        /// </summary>
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
