namespace VoyageAI.API.DTOs.Activities
{
    /// <summary>
    /// DTO for returning an activity response to the client.
    /// 
    /// This response is sent to the client for:
    /// - POST /api/trips/{tripId}/itinerary/{dayId}/activities - Create activity returns this
    /// - GET /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Get single activity details
    /// - PUT /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Update activity returns this
    /// 
    /// Includes all fields including audit trail for transparency and API completeness.
    /// 
    /// Convention:
    /// - DateTime fields are in ISO 8601 format in UTC
    /// - TimeOnly fields are in HH:mm:ss format
    /// - All monetary values use the trip's currency
    /// - Guids are in standard format (8-4-4-4-12 hex)
    /// - Enum fields are returned as integer values (matching request format)
    /// 
    /// Audit Trail Information:
    /// - CreatedAt: When the activity was created
    /// - UpdatedAt: When the activity was last modified
    /// - CreatedBy: UserId of who created the activity
    /// - LastModifiedBy: UserId of who last modified the activity
    /// - IsDeleted: Whether the activity is soft-deleted (false for normal queries)
    /// - DeletedAt: When the activity was deleted (null for active activities)
    /// 
    /// Usage:
    /// This DTO includes ALL information about the activity and is used for detailed views.
    /// For list views with many activities, a summary DTO might be more appropriate.
    /// </summary>
    public class ActivityResponse
    {
        /// <summary>
        /// Unique identifier for this activity.
        /// Used for update and delete operations.
        /// Format: GUID (e.g., "550e8400-e29b-41d4-a716-446655440000")
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// The itinerary day this activity belongs to.
        /// Used for back-references and navigation.
        /// </summary>
        public Guid DayId { get; set; }

        /// <summary>
        /// Name/title of the activity.
        /// </summary>
        public string ActivityName { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the activity.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of the activity (integer enum value).
        /// 0=Sightseeing, 1=Restaurant, 2=Museum, 3=Shopping, 4=Transportation, 
        /// 5=Adventure, 6=Entertainment, 7=Hotel, 8=Flight, 9=Other
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Name of the location where the activity takes place.
        /// </summary>
        public string LocationName { get; set; } = string.Empty;

        /// <summary>
        /// Street address of the activity location.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Latitude of the activity location.
        /// Between -90 and 90 (degrees).
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude of the activity location.
        /// Between -180 and 180 (degrees).
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Start time of the activity on the day.
        /// Format: TimeOnly (HH:mm:ss)
        /// Example: "09:30:00"
        /// </summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// End time of the activity on the day.
        /// Format: TimeOnly (HH:mm:ss)
        /// Example: "11:30:00"
        /// </summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// Estimated cost of the activity in the trip's currency.
        /// Default: 0.
        /// </summary>
        public decimal EstimatedCost { get; set; }

        /// <summary>
        /// Actual cost spent on the activity.
        /// Default: 0.
        /// Populated after the activity is completed.
        /// </summary>
        public decimal ActualCost { get; set; }

        /// <summary>
        /// Booking reference/confirmation number for the activity.
        /// Empty string if not booked.
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>
        /// Website URL of the activity/venue/booking platform.
        /// Empty string if not available.
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the activity/venue.
        /// Empty string if not available.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes about the activity.
        /// Empty string if no notes.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Priority level of this activity (integer enum value).
        /// 0=Low, 1=Medium, 2=High, 3=MustVisit
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Current status of the activity (integer enum value).
        /// 0=Planned, 1=Booked, 2=Completed, 3=Cancelled, 4=Skipped
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Duration of the activity in minutes.
        /// Calculated from StartTime and EndTime.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// URL of an image for the activity/location.
        /// Empty string if not available.
        /// Used for UI display and preview.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this activity was AI-generated.
        /// true if auto-generated by AI optimization/suggestions.
        /// false if manually added by user.
        /// </summary>
        public bool IsAiGenerated { get; set; }

        /// <summary>
        /// Indicates if this activity record is deleted (soft delete).
        /// true if activity is soft-deleted.
        /// false for normal active activities.
        /// Note: Deleted activities are not returned in normal queries.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// UTC timestamp when the activity was deleted.
        /// Null if not deleted.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the activity was created.
        /// Format: ISO 8601 (e.g., "2025-06-15T09:30:00Z")
        /// Immutable after creation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the activity was last modified.
        /// Format: ISO 8601 (e.g., "2025-06-15T10:45:00Z")
        /// Updated every time the activity is changed.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// UserId of the user who created this activity.
        /// Format: GUID
        /// Immutable after creation.
        /// Used for audit trail.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// UserId of the user who last modified this activity.
        /// Format: GUID
        /// Updated every time the activity is changed.
        /// Used for audit trail.
        /// </summary>
        public Guid LastModifiedBy { get; set; }
    }
}
