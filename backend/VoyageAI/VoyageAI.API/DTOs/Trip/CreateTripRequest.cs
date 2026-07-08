namespace VoyageAI.API.DTOs.Trip
{
    /// <summary>
    /// DTO for creating a new trip.
    /// This request is sent by authenticated users to create a new trip.
    /// The UserId is extracted from the JWT token, so it's not included in the request.
    /// </summary>
    public class CreateTripRequest
    {
        /// <summary>
        /// The name of the trip (e.g., "Summer 2025 Europe Adventure").
        /// </summary>
        public string TripName { get; set; } = string.Empty;

        /// <summary>
        /// The destination country (required field).
        /// </summary>
        public string DestinationCountry { get; set; } = string.Empty;

        /// <summary>
        /// The destination city (required field).
        /// </summary>
        public string DestinationCity { get; set; } = string.Empty;

        /// <summary>
        /// The start date of the trip (inclusive).
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the trip (inclusive).
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The budget for the trip in the specified currency.
        /// </summary>
        public decimal Budget { get; set; }

        /// <summary>
        /// The currency code for the budget (e.g., "USD", "EUR", "GBP").
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// The travel style (e.g., "Budget", "Comfort", "Luxury", "Adventure").
        /// </summary>
        public string TravelStyle { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the trip.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional URL to a cover image for the trip.
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// Initial status of the trip (e.g., "Planning", "Confirmed", "Completed").
        /// Defaults to "Planning".
        /// </summary>
        public string Status { get; set; } = "Planning";
    }
}
