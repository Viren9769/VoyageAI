namespace VoyageAI.API.Models.Entities
{
    /// <summary>
    /// Activity entity representing a planned activity within an itinerary day.
    /// 
    /// Each Activity belongs to exactly ONE ItineraryDay.
    /// Each ItineraryDay can contain multiple Activities.
    /// 
    /// An activity represents a specific action/event planned for a trip:
    /// - Restaurant reservation
    /// - Museum visit
    /// - Flight transfer
    /// - Adventure activity
    /// - Hotel check-in
    /// - etc.
    /// 
    /// Soft Delete Pattern:
    /// - Activities are never physically deleted
    /// - Instead, IsDeleted is set to true and DeletedAt records the time
    /// - Queries must filter out deleted activities
    /// 
    /// Audit Fields:
    /// - CreatedAt: When the activity was created
    /// - UpdatedAt: When the activity was last modified
    /// - CreatedBy: UserId of who created the activity
    /// - LastModifiedBy: UserId of who last modified the activity
    /// 
    /// Relationships:
    /// - Belongs to ItineraryDay (via DayId)
    /// - Indirectly related to Trip (through ItineraryDay)
    /// - Indirectly related to User (through Trip ownership)
    /// 
    /// Database Indexes:
    /// - (DayId)
    /// - (Category)
    /// - (Status)
    /// - (Priority)
    /// - (StartTime)
    /// - (DayId, IsDeleted)
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Unique identifier for the activity.
        /// Primary key.
        /// </summary>
        public Guid ActivityId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Foreign key to the ItineraryDay this activity belongs to.
        /// CRITICAL: Every activity must belong to exactly ONE itinerary day.
        /// </summary>
        public Guid DayId { get; set; }

        /// <summary>
        /// Name/title of the activity (e.g., "Visit Eiffel Tower", "Dinner at Le Jules Verne").
        /// Required. Max length: 255 characters.
        /// </summary>
        public string ActivityName { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the activity.
        /// Optional. Provides context and details.
        /// Max length: 1000 characters.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of the activity (e.g., Sightseeing, Restaurant, Museum, Transportation).
        /// Required. Must be one of ActivityCategory enum values.
        /// Stored as enum value (integer).
        /// Used for filtering, search, and cost analysis.
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Name of the location where the activity takes place.
        /// Optional. Human-readable location name.
        /// Max length: 255 characters.
        /// Example: "Eiffel Tower", "Restaurant Le Jules Verne", "Charles de Gaulle Airport"
        /// </summary>
        public string LocationName { get; set; } = string.Empty;

        /// <summary>
        /// Street address of the activity location.
        /// Optional. Full address for navigation and booking.
        /// Max length: 500 characters.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Latitude of the activity location.
        /// Optional. Used for mapping and route optimization.
        /// Must be between -90 and 90 (degrees).
        /// Default: 0 if not provided.
        /// </summary>
        public decimal Latitude { get; set; } = 0;

        /// <summary>
        /// Longitude of the activity location.
        /// Optional. Used for mapping and route optimization.
        /// Must be between -180 and 180 (degrees).
        /// Default: 0 if not provided.
        /// </summary>
        public decimal Longitude { get; set; } = 0;

        /// <summary>
        /// Start time of the activity on the day.
        /// Required. Must be in TimeOnly format (no date).
        /// Example: 09:30:00, 14:00:00, etc.
        /// </summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// End time of the activity on the day.
        /// Required. Must be after StartTime.
        /// Used to calculate DurationMinutes.
        /// </summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// Estimated cost of the activity in the trip's currency.
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// Used for budget planning and tracking.
        /// Example: 15.50 for a museum entry fee.
        /// </summary>
        public decimal EstimatedCost { get; set; } = 0;

        /// <summary>
        /// Actual cost spent on the activity (recorded after completion).
        /// Optional. Default: 0.
        /// Must be >= 0.
        /// Updated after trip completion with real expenses.
        /// Helps with expense tracking and budget analysis.
        /// </summary>
        public decimal ActualCost { get; set; } = 0;

        /// <summary>
        /// Booking reference/confirmation number for the activity.
        /// Optional. Used when the activity is booked.
        /// Examples: "RES-12345", "CONF-ABC-789", "TKT-REF-452"
        /// Max length: 100 characters.
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>
        /// Website URL of the activity/venue/booking platform.
        /// Optional. Useful for looking up info or making reservations.
        /// Max length: 500 characters.
        /// Must be a valid URL format.
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the activity/venue.
        /// Optional. Contact number for inquiries or changes.
        /// Max length: 20 characters.
        /// Can include country code and formatting.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes about the activity.
        /// Optional. Any special instructions, requirements, or comments.
        /// Max length: 1000 characters.
        /// Examples: "Arrive 15 minutes early", "Bring passport", "Vegetarian options available"
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Priority level of this activity.
        /// Required. One of: Low, Medium, High, MustVisit.
        /// Stored as enum value (integer).
        /// Used for itinerary optimization and filtering.
        /// Higher priority activities should be prioritized if constraints arise.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Current status of the activity.
        /// Required. One of: Planned, Booked, Completed, Cancelled, Skipped.
        /// Stored as enum value (integer).
        /// Default when created: Planned
        /// Tracks the lifecycle of the activity through the trip.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Duration of the activity in minutes.
        /// Calculated from StartTime and EndTime.
        /// Optional. Can be auto-calculated from times.
        /// Used for schedule visualization and time planning.
        /// </summary>
        public int DurationMinutes { get; set; } = 0;

        /// <summary>
        /// URL of an image for the activity/location.
        /// Optional. Profile photo or location photo.
        /// Used for UI display and preview.
        /// Max length: 500 characters.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this activity was AI-generated.
        /// Default: false.
        /// true if auto-generated by AI optimization/suggestions.
        /// false if manually added by user.
        /// </summary>
        public bool IsAiGenerated { get; set; } = false;

        // ============================================================
        // SOFT DELETE & AUDIT FIELDS
        // ============================================================

        /// <summary>
        /// Indicates if this activity record is deleted.
        /// Soft delete pattern: IsDeleted = true instead of physical deletion.
        /// Default: false.
        /// 
        /// Index: (DayId, IsDeleted) for efficient querying of active activities.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// UTC timestamp when the activity was deleted.
        /// Null if not deleted.
        /// Only set when IsDeleted becomes true.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the activity was created.
        /// Set automatically when the activity is first created.
        /// Never changes after initial creation.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UTC timestamp when the activity was last modified.
        /// Updated every time the activity is changed.
        /// Initially same as CreatedAt.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UserId of the user who created this activity.
        /// Set automatically from the authenticated user's claims.
        /// Never changes after initial creation.
        /// Used for audit trail and authorization.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// UserId of the user who last modified this activity.
        /// Updated every time the activity is changed.
        /// Initially same as CreatedBy.
        /// Used for audit trail.
        /// </summary>
        public Guid LastModifiedBy { get; set; }

        // ============================================================
        // NAVIGATION PROPERTIES
        // ============================================================

        /// <summary>
        /// Navigation property to the ItineraryDay this activity belongs to.
        /// Required relationship: Activity -> ItineraryDay
        /// Loaded via .Include(a => a.ItineraryDay) in queries.
        /// </summary>
        public ItineraryDay ItineraryDay { get; set; }
    }
}

