namespace VoyageAI.API.DTOs.Trip
{
    /// <summary>
    /// DTO for returning trip information in API responses.
    /// This is used when retrieving trips from the API.
    /// </summary>
    public class GetTripResponse
    {
        /// <summary>
        /// The unique identifier of the trip.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        /// The user ID who owns this trip.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string TripName { get; set; } = string.Empty;

        /// <summary>
        /// The destination country.
        /// </summary>
        public string DestinationCountry { get; set; } = string.Empty;

        /// <summary>
        /// The destination city.
        /// </summary>
        public string DestinationCity { get; set; } = string.Empty;

        /// <summary>
        /// The start date of the trip.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the trip.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The budget amount for the trip.
        /// </summary>
        public decimal Budget { get; set; }

        /// <summary>
        /// The currency code for the budget.
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// The travel style (Budget, Comfort, Luxury, Adventure).
        /// </summary>
        public string TravelStyle { get; set; } = string.Empty;

        /// <summary>
        /// The description of the trip.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL to the cover image for the trip.
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// The current status of the trip (Planning, Confirmed, Completed, Cancelled).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// When the trip was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the trip was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Duration of the trip in days.
        /// Calculated as (EndDate - StartDate).TotalDays.
        /// </summary>
        public int DurationDays => (int)((EndDate - StartDate).TotalDays + 1);
    }
}
