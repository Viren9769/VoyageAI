using FluentValidation;
using VoyageAI.API.DTOs.Activities;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for UpdateActivityRequest.
    /// 
    /// This validator ensures that when a user updates an activity, all provided fields
    /// (if not null) are valid. FluentValidation provides a fluent, testable,
    /// and composable way to define validation rules.
    /// 
    /// Validation Rules (all fields optional, but if provided must be valid):
    /// 1. ActivityName - If provided, 1-255 characters
    /// 2. Category - If provided, must be valid ActivityCategory enum (0-9)
    /// 3. StartTime - If provided, must be valid TimeOnly
    /// 4. EndTime - If provided, must be after StartTime
    /// 5. Priority - If provided, must be valid Priority enum (0-3)
    /// 6. Status - If provided, must be valid ActivityStatus enum (0-4)
    /// 7. Latitude - If provided, must be between -90 and 90
    /// 8. Longitude - If provided, must be between -180 and 180
    /// 9. EstimatedCost - If provided, must be >= 0
    /// 10. ActualCost - If provided, must be >= 0
    /// 11. Website - If provided, must be valid URL
    /// 12. ImageUrl - If provided, must be valid URL
    /// 13. Other optional fields - Max length validation
    /// 
    /// Partial Updates:
    /// - All properties are nullable in UpdateActivityRequest
    /// - Validation only applies to non-null values
    /// - This allows clients to update individual fields
    /// 
    /// Note: Business logic validation (time conflicts) happens in the service layer 
    /// because we need to check against existing activities in the database.
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// 
    /// Usage in Controller:
    /// The [ApiController] attribute automatically validates requests
    /// and returns 400 Bad Request if validation fails.
    /// </summary>
    public class UpdateActivityRequestValidator : AbstractValidator<UpdateActivityRequest>
    {
        /// <summary>
        /// Initializes the validator with all validation rules for UpdateActivityRequest.
        /// </summary>
        public UpdateActivityRequestValidator()
        {
            // ActivityName validation (optional, but if provided must be valid)
            RuleFor(x => x.ActivityName)
                .Length(1, 255)
                .WithMessage("Activity name must be between 1 and 255 characters")
                .When(x => !string.IsNullOrEmpty(x.ActivityName));

            // Category validation (optional, but if provided must be valid)
            RuleFor(x => x.Category)
                .InclusiveBetween(0, 9)
                .WithMessage("Category must be a valid ActivityCategory (0-9)")
                .When(x => x.Category.HasValue);

            // Priority validation (optional, but if provided must be valid)
            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 3)
                .WithMessage("Priority must be a valid Priority level (0-3: Low, Medium, High, MustVisit)")
                .When(x => x.Priority.HasValue);

            // Status validation (optional, but if provided must be valid)
            RuleFor(x => x.Status)
                .InclusiveBetween(0, 4)
                .WithMessage("Status must be a valid ActivityStatus (0-4: Planned, Booked, Completed, Cancelled, Skipped)")
                .When(x => x.Status.HasValue);

            // StartTime validation (optional)
            // No specific validation needed for TimeOnly, just accept the value

            // EndTime validation (must be after StartTime if both are provided)
            RuleFor(x => x.EndTime)
                .Must((request, endTime) => !endTime.HasValue || !request.StartTime.HasValue || endTime > request.StartTime)
                .WithMessage("End time must be after start time")
                .When(x => x.EndTime.HasValue || x.StartTime.HasValue);

            // Latitude validation (optional, but if provided must be in range)
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90")
                .When(x => x.Latitude.HasValue);

            // Longitude validation (optional, but if provided must be in range)
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180")
                .When(x => x.Longitude.HasValue);

            // EstimatedCost validation (optional, but if provided must be valid)
            RuleFor(x => x.EstimatedCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Estimated cost must be greater than or equal to 0")
                .When(x => x.EstimatedCost.HasValue);

            // ActualCost validation (optional, but if provided must be valid)
            RuleFor(x => x.ActualCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Actual cost must be greater than or equal to 0")
                .When(x => x.ActualCost.HasValue);

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

            // Description validation (optional, but if provided must be valid)
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // LocationName validation (optional, but if provided must be valid)
            RuleFor(x => x.LocationName)
                .MaximumLength(255)
                .WithMessage("Location name must not exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.LocationName));

            // Address validation (optional, but if provided must be valid)
            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            // BookingReference validation (optional, but if provided must be valid)
            RuleFor(x => x.BookingReference)
                .MaximumLength(100)
                .WithMessage("Booking reference must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.BookingReference));

            // Phone validation (optional, but if provided must be valid)
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            // Notes validation (optional, but if provided must be valid)
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
