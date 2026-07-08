using FluentValidation;
using VoyageAI.API.DTOs.Trip;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for CreateTripRequest.
    /// 
    /// This validator ensures that when a user creates a new trip, all required fields
    /// are provided and have valid values. FluentValidation provides a fluent, testable,
    /// and composable way to define validation rules.
    /// 
    /// Validation Rules:
    /// 1. TripName - Required, 1-200 characters
    /// 2. DestinationCountry - Required, 1-100 characters
    /// 3. DestinationCity - Required, 1-100 characters
    /// 4. StartDate - Required, must not be in the past
    /// 5. EndDate - Required, must not be in the past, must be on or after StartDate
    /// 6. Budget - Required, must be greater than 0
    /// 7. Currency - Required, must be 1-10 characters
    /// 8. TravelStyle - Required, must be 1-50 characters
    /// 9. Description - Optional, max 1000 characters
    /// 10. CoverImageUrl - Optional, valid URL format
    /// 11. Status - Required, must be a valid status value
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// 
    /// Usage in Controller:
    /// The [ApiController] attribute automatically validates requests
    /// and returns 400 Bad Request if validation fails.
    /// </summary>
    public class CreateTripRequestValidator : AbstractValidator<CreateTripRequest>
    {
        public CreateTripRequestValidator()
        {
            // TripName validation
            RuleFor(x => x.TripName)
                .NotEmpty()
                .WithMessage("Trip name is required")
                .MaximumLength(200)
                .WithMessage("Trip name must not exceed 200 characters");

            // DestinationCountry validation
            RuleFor(x => x.DestinationCountry)
                .NotEmpty()
                .WithMessage("Destination country is required")
                .MaximumLength(100)
                .WithMessage("Destination country must not exceed 100 characters");

            // DestinationCity validation
            RuleFor(x => x.DestinationCity)
                .NotEmpty()
                .WithMessage("Destination city is required")
                .MaximumLength(100)
                .WithMessage("Destination city must not exceed 100 characters");

            // StartDate validation
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Start date must not be in the past");

            // EndDate validation
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("End date must not be in the past")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after start date");

            // Budget validation
            RuleFor(x => x.Budget)
                .GreaterThan(0)
                .WithMessage("Budget must be greater than 0");

            // Currency validation
            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(1, 10)
                .WithMessage("Currency code must be 1-10 characters");

            // TravelStyle validation
            RuleFor(x => x.TravelStyle)
                .NotEmpty()
                .WithMessage("Travel style is required")
                .MaximumLength(50)
                .WithMessage("Travel style must not exceed 50 characters");

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

            // Status validation
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required")
                .Must(BeAValidStatus)
                .WithMessage("Status must be one of: Planning, Confirmed, Completed, Cancelled");
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
