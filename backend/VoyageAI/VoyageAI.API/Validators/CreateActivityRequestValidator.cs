using FluentValidation;
using VoyageAI.API.DTOs.Activities;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for CreateActivityRequest.
    /// 
    /// This validator ensures that when a user creates a new activity, all required fields
    /// are provided and have valid values. FluentValidation provides a fluent, testable,
    /// and composable way to define validation rules.
    /// 
    /// Validation Rules:
    /// 1. ActivityName - Required, 1-255 characters
    /// 2. Category - Required, must be valid ActivityCategory enum (0-9)
    /// 3. StartTime - Required, must be valid TimeOnly
    /// 4. EndTime - Required, must be after StartTime
    /// 5. Priority - Required, must be valid Priority enum (0-3)
    /// 6. Status - Optional, if provided must be valid ActivityStatus enum (0-4)
    /// 7. Latitude - Optional, must be between -90 and 90 if provided
    /// 8. Longitude - Optional, must be between -180 and 180 if provided
    /// 9. EstimatedCost - Optional, must be >= 0 if provided
    /// 10. ActualCost - Optional, must be >= 0 if provided
    /// 11. Website - Optional, must be valid URL if provided
    /// 12. ImageUrl - Optional, must be valid URL if provided
    /// 13. Other optional fields - Max length validation
    /// 
    /// Note: Business logic validation (duplicate activity names, time conflicts, 
    /// activity name uniqueness per day) happens in the service layer because we need
    /// database access and other context to validate these.
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// 
    /// Usage in Controller:
    /// The [ApiController] attribute automatically validates requests
    /// and returns 400 Bad Request if validation fails.
    /// </summary>
    public class CreateActivityRequestValidator : AbstractValidator<CreateActivityRequest>
    {
        /// <summary>
        /// Initializes the validator with all validation rules for CreateActivityRequest.
        /// </summary>
        public CreateActivityRequestValidator()
        {
            // ActivityName validation
            RuleFor(x => x.ActivityName)
                .NotEmpty()
                .WithMessage("Activity name is required")
                .Length(1, 255)
                .WithMessage("Activity name must be between 1 and 255 characters");

            // Category validation
            RuleFor(x => x.Category)
                .InclusiveBetween(0, 9)
                .WithMessage("Category must be a valid ActivityCategory (0-9)");

            // Priority validation
            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 3)
                .WithMessage("Priority must be a valid Priority level (0-3: Low, Medium, High, MustVisit)");

            // Status validation (optional, but if provided must be valid)
            RuleFor(x => x.Status)
                .InclusiveBetween(0, 4)
                .WithMessage("Status must be a valid ActivityStatus (0-4: Planned, Booked, Completed, Cancelled, Skipped)")
                .When(x => x.Status.HasValue);

            // StartTime validation
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required");

            // EndTime validation
            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("End time is required")
                .Must((request, endTime) => endTime > request.StartTime)
                .WithMessage("End time must be after start time");

            // Latitude validation
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90")
                .When(x => x.Latitude.HasValue);

            // Longitude validation
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180")
                .When(x => x.Longitude.HasValue);

            // EstimatedCost validation
            RuleFor(x => x.EstimatedCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Estimated cost must be greater than or equal to 0");

            // ActualCost validation
            RuleFor(x => x.ActualCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Actual cost must be greater than or equal to 0");

            // Website validation (must be valid URL if provided)
            RuleFor(x => x.Website)
                .Must(BeAValidUrl)
                .WithMessage("Website must be a valid URL (e.g., https://example.com)")
                .When(x => !string.IsNullOrEmpty(x.Website));

            // ImageUrl validation (must be valid URL if provided)
            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl)
                .WithMessage("Image URL must be a valid URL (e.g., https://example.com/image.jpg)")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            // Description validation
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // LocationName validation
            RuleFor(x => x.LocationName)
                .MaximumLength(255)
                .WithMessage("Location name must not exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.LocationName));

            // Address validation
            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            // BookingReference validation
            RuleFor(x => x.BookingReference)
                .MaximumLength(100)
                .WithMessage("Booking reference must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.BookingReference));

            // Phone validation
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            // Notes validation
            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }

        /// <summary>
        /// Validates that a string is a valid URL.
        /// Accepts http://, https://, ftp://, and file:// schemes.
        /// </summary>
        /// <param name="url">The URL string to validate</param>
        /// <returns>true if URL is valid, false otherwise</returns>
        private static bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true; // Allow null/empty (handled by required rule if needed)

            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == "http" || result.Scheme == "https" || 
                    result.Scheme == "ftp" || result.Scheme == "file");
        }
    }
}
