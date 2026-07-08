using FluentValidation;
using VoyageAI.API.DTOs.Trip;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for UpdateTripRequest.
    /// 
    /// This validator ensures that when a user updates a trip, all provided fields
    /// have valid values. Unlike CreateTripRequestValidator, all fields are optional
    /// (nullable) since this is a partial update - users can update just the fields they need.
    /// 
    /// Validation Rules (all optional):
    /// 1. TripName - If provided, 1-200 characters
    /// 2. DestinationCountry - If provided, 1-100 characters
    /// 3. DestinationCity - If provided, 1-100 characters
    /// 4. StartDate - If provided, must not be in the past
    /// 5. EndDate - If provided, must not be in the past
    /// 6. Budget - If provided, must be greater than 0
    /// 7. Currency - If provided, must be 1-10 characters
    /// 8. TravelStyle - If provided, must be 1-50 characters
    /// 9. Description - If provided, max 1000 characters
    /// 10. CoverImageUrl - If provided, valid URL format
    /// 11. Status - If provided, must be a valid status value
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// </summary>
    public class UpdateTripRequestValidator : AbstractValidator<UpdateTripRequest>
    {
        public UpdateTripRequestValidator()
        {
            // TripName validation (optional)
            RuleFor(x => x.TripName)
                .MaximumLength(200)
                .WithMessage("Trip name must not exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TripName));

            // DestinationCountry validation (optional)
            RuleFor(x => x.DestinationCountry)
                .MaximumLength(100)
                .WithMessage("Destination country must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DestinationCountry));

            // DestinationCity validation (optional)
            RuleFor(x => x.DestinationCity)
                .MaximumLength(100)
                .WithMessage("Destination city must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DestinationCity));

            // StartDate validation (optional)
            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Start date must not be in the past")
                .When(x => x.StartDate.HasValue);

            // EndDate validation (optional)
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("End date must not be in the past")
                .When(x => x.EndDate.HasValue);

            // Conditional validation: if both dates are provided, EndDate >= StartDate
            RuleFor(x => x)
                .Must(HaveValidDateRange)
                .WithMessage("End date must be on or after start date")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            // Budget validation (optional)
            RuleFor(x => x.Budget)
                .GreaterThan(0)
                .WithMessage("Budget must be greater than 0")
                .When(x => x.Budget.HasValue);

            // Currency validation (optional)
            RuleFor(x => x.Currency)
                .Length(1, 10)
                .WithMessage("Currency code must be 1-10 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Currency));

            // TravelStyle validation (optional)
            RuleFor(x => x.TravelStyle)
                .MaximumLength(50)
                .WithMessage("Travel style must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TravelStyle));

            // Description validation (optional)
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            // CoverImageUrl validation (optional)
            RuleFor(x => x.CoverImageUrl)
                .Must(BeAValidUrl)
                .WithMessage("Cover image URL must be a valid URL")
                .When(x => !string.IsNullOrWhiteSpace(x.CoverImageUrl));

            // Status validation (optional)
            RuleFor(x => x.Status)
                .Must(BeAValidStatus)
                .WithMessage("Status must be one of: Planning, Confirmed, Completed, Cancelled")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));
        }

        /// <summary>
        /// Validates that the date range is valid (EndDate >= StartDate).
        /// </summary>
        private static bool HaveValidDateRange(UpdateTripRequest request)
        {
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                return true;

            return request.EndDate >= request.StartDate;
        }

        /// <summary>
        /// Validates that a given string is a valid URL.
        /// </summary>
        private static bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        /// <summary>
        /// Validates that a status is one of the allowed values.
        /// </summary>
        private static bool BeAValidStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            var validStatuses = new[] { "Planning", "Confirmed", "Completed", "Cancelled" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
