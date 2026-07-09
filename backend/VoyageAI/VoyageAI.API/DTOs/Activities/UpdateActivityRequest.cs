using System.ComponentModel.DataAnnotations;

namespace VoyageAI.API.DTOs.Activities
{
    /// <summary>
    /// DTO for updating an existing activity within an itinerary day.
    /// This request is sent by authenticated users to modify an activity in their itinerary day.
    /// 
    /// Security:
    /// - UserId is extracted from JWT token, not from request
    /// - User must own the trip (verified through itinerary day ownership)
    /// - Can only update activities that belong to their own itinerary days
    /// 
    /// Validation:
    /// - All fields are optional (allows partial updates)
    /// - ActivityName: If provided, max 255 characters
    /// - Category: If provided, must be valid ActivityCategory enum value
    /// - StartTime: If provided, must be valid TimeOnly
    /// - EndTime: If provided, must be after StartTime
    /// - Latitude: If provided, must be between -90 and 90
    /// - Longitude: If provided, must be between -180 and 180
    /// - EstimatedCost: If provided, must be >= 0
    /// - ActualCost: If provided, must be >= 0
    /// - Website: If provided, must be valid URL
    /// - Priority: If provided, must be valid Priority enum value
    /// - Status: If provided, must be valid ActivityStatus enum value
    /// 
    /// Validation Layers:
    /// - DataAnnotations: Basic validation in this DTO
    /// - FluentValidation: Complex validation in UpdateActivityRequestValidator
    /// - Service Layer: Business logic validation (duplicate checking, time conflicts, etc.)
    /// 
    /// Partial Updates:
    /// - If a property is null/not provided, that field is not updated
    /// - Only non-null properties are applied to the existing entity
    /// - This allows clients to update multiple fields or just one field
    /// </summary>
    public class UpdateActivityRequest
    {
        /// <summary>
        /// Name/title of the activity (e.g., "Visit Eiffel Tower", "Dinner at Le Jules Verne").
        /// Optional. If provided, max length: 255 characters.
        /// If null/not provided, existing value is not changed.
        /// If provided, cannot be empty string.
        /// </summary>
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Activity name must be between 1 and 255 characters")]
        public string? ActivityName { get; set; }

        /// <summary>
        /// Detailed description of the activity.
        /// Optional. Max length: 1000 characters.
        /// If null/not provided, existing value is not changed.
        /// Provides additional context and details about what the activity entails.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Category of the activity (e.g., Sightseeing, Restaurant, Museum, Transportation).
        /// Optional. If provided, must be one of ActivityCategory enum values.
        /// Valid values: 0=Sightseeing, 1=Restaurant, 2=Museum, 3=Shopping, 4=Transportation, 
        ///              5=Adventure, 6=Entertainment, 7=Hotel, 8=Flight, 9=Other
        /// If null/not provided, existing value is not changed.
        /// Used for filtering, search, and cost analysis.
        /// </summary>
        [Range(0, 9, ErrorMessage = "Category must be a valid ActivityCategory")]
        public int? Category { get; set; }

        /// <summary>
        /// Name of the location where the activity takes place.
        /// Optional. Human-readable location name.
        /// Max length: 255 characters.
        /// If null/not provided, existing value is not changed.
        /// Example: "Eiffel Tower", "Restaurant Le Jules Verne", "Charles de Gaulle Airport"
        /// </summary>
        [StringLength(255, ErrorMessage = "Location name must not exceed 255 characters")]
        public string? LocationName { get; set; }

        /// <summary>
        /// Street address of the activity location.
        /// Optional. Full address for navigation and booking.
        /// Max length: 500 characters.
        /// If null/not provided, existing value is not changed.
        /// Example: "5 Avenue Anatole France, 75007 Paris, France"
        /// </summary>
        [StringLength(500, ErrorMessage = "Address must not exceed 500 characters")]
        public string? Address { get; set; }

        /// <summary>
        /// Latitude of the activity location.
        /// Optional. Used for mapping and route optimization.
        /// Must be between -90 and 90 (degrees).
        /// If null/not provided, existing value is not changed.
        /// Example: 48.8584 for Eiffel Tower
        /// </summary>
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude of the activity location.
        /// Optional. Used for mapping and route optimization.
        /// Must be between -180 and 180 (degrees).
        /// If null/not provided, existing value is not changed.
        /// Example: 2.2945 for Eiffel Tower
        /// </summary>
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Start time of the activity on the day.
        /// Optional. Must be in TimeOnly format (no date).
        /// Example: "09:30:00", "14:00:00", etc.
        /// Format: "HH:mm:ss" or "HH:mm"
        /// If provided, must be before EndTime (validated by service layer).
        /// If null/not provided, existing value is not changed.
        /// </summary>
        public TimeOnly? StartTime { get; set; }

        /// <summary>
        /// End time of the activity on the day.
        /// Optional. Must be after StartTime.
        /// Format: "HH:mm:ss" or "HH:mm"
        /// Used to calculate DurationMinutes.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        public TimeOnly? EndTime { get; set; }

        /// <summary>
        /// Estimated cost of the activity in the trip's currency.
        /// Optional. Must be >= 0 if provided.
        /// Used for budget planning and tracking.
        /// Example: 15.50 for a museum entry fee.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Estimated cost must be greater than or equal to 0")]
        public decimal? EstimatedCost { get; set; }

        /// <summary>
        /// Actual cost spent on the activity (recorded after completion).
        /// Optional. Must be >= 0 if provided.
        /// Updated after trip completion with real expenses.
        /// Can be provided at creation or updated later.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Actual cost must be greater than or equal to 0")]
        public decimal? ActualCost { get; set; }

        /// <summary>
        /// Booking reference/confirmation number for the activity.
        /// Optional. Used when the activity is booked.
        /// Examples: "RES-12345", "CONF-ABC-789", "TKT-REF-452"
        /// Max length: 100 characters.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [StringLength(100, ErrorMessage = "Booking reference must not exceed 100 characters")]
        public string? BookingReference { get; set; }

        /// <summary>
        /// Website URL of the activity/venue/booking platform.
        /// Optional. Useful for looking up info or making reservations.
        /// Max length: 500 characters.
        /// Must follow URL format if provided (validated by FluentValidation).
        /// Example: "https://www.toureiffel.paris.fr"
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [StringLength(500, ErrorMessage = "Website must not exceed 500 characters")]
        public string? Website { get; set; }

        /// <summary>
        /// Phone number of the activity/venue.
        /// Optional. Contact number for inquiries or changes.
        /// Max length: 20 characters.
        /// Can include country code and formatting.
        /// Example: "+33 1 45 55 75 01"
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
        public string? Phone { get; set; }

        /// <summary>
        /// Additional notes about the activity.
        /// Optional. Any special instructions, requirements, or comments.
        /// Max length: 1000 characters.
        /// Examples: "Arrive 15 minutes early", "Bring passport", "Vegetarian options available"
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
        public string? Notes { get; set; }

        /// <summary>
        /// Priority level of this activity.
        /// Optional. One of: 0=Low, 1=Medium, 2=High, 3=MustVisit.
        /// Used for itinerary optimization and filtering.
        /// Higher priority activities should be prioritized if constraints arise.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [Range(0, 3, ErrorMessage = "Priority must be a valid Priority level")]
        public int? Priority { get; set; }

        /// <summary>
        /// Current status of the activity.
        /// Optional. One of: 0=Planned, 1=Booked, 2=Completed, 3=Cancelled, 4=Skipped.
        /// Tracks the lifecycle of the activity through the trip.
        /// Can be updated as the user progresses through their itinerary.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [Range(0, 4, ErrorMessage = "Status must be a valid ActivityStatus")]
        public int? Status { get; set; }

        /// <summary>
        /// URL of an image for the activity/location.
        /// Optional. Profile photo or location photo.
        /// Used for UI display and preview.
        /// Max length: 500 characters.
        /// Must follow URL format if provided (validated by FluentValidation).
        /// If null/not provided, existing value is not changed.
        /// </summary>
        [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Indicates if this activity is AI-generated.
        /// Optional. If provided, updates the flag.
        /// true if auto-generated by AI optimization/suggestions.
        /// false if manually added by user.
        /// If null/not provided, existing value is not changed.
        /// </summary>
        public bool? IsAiGenerated { get; set; }
    }
}
