using FluentValidation;
using VoyageAI.API.DTOs.Itinerary;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for UpdateItineraryDayRequest.
    /// 
    /// This validator ensures that when a user updates an existing itinerary day, 
    /// all fields have valid values. FluentValidation provides a fluent, testable,
    /// and composable way to define validation rules.
    /// 
    /// Validation Rules:
    /// 1. DayNumber - Required, must be > 0
    /// 2. Date - Required, must be valid DateTime
    /// 3. Title - Required, 1-200 characters
    /// 4. Summary - Optional, max 1000 characters
    /// 5. Notes - Optional, max 3000 characters
    /// 6. EstimatedBudget - Must be >= 0
    /// 7. ActualBudget - Must be >= 0
    /// 8. WeatherSummary - Optional, max 500 characters
    /// 
    /// Differences from Create Validator:
    /// - Both require the same field validation
    /// - DayNumber uniqueness validation (excluding current day) happens in service layer
    /// - Date range validation (must fall within trip dates) happens in service layer
    /// 
    /// Note: Date range validation (must fall within trip dates) happens in the service layer
    /// because we need to load the trip to validate against its StartDate/EndDate.
    /// 
    /// Note: DayNumber uniqueness validation happens in the service layer
    /// because we need to check against other days in the trip (excluding current day).
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// 
    /// Usage in Controller:
    /// The [ApiController] attribute automatically validates requests
    /// and returns 400 Bad Request if validation fails.
    /// </summary>
    public class UpdateItineraryDayRequestValidator : AbstractValidator<UpdateItineraryDayRequest>
    {
        public UpdateItineraryDayRequestValidator()
        {
            // DayNumber validation
            RuleFor(x => x.DayNumber)
                .NotEmpty()
                .WithMessage("Day number is required")
                .GreaterThan(0)
                .WithMessage("Day number must be greater than 0");

            // Date validation
            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .Must(d => d.Kind == DateTimeKind.Utc || d.Kind == DateTimeKind.Unspecified)
                .WithMessage("Date should be in UTC format");

            // Title validation
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters");

            // Summary validation
            RuleFor(x => x.Summary)
                .MaximumLength(1000)
                .WithMessage("Summary must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Summary));

            // Notes validation
            RuleFor(x => x.Notes)
                .MaximumLength(3000)
                .WithMessage("Notes must not exceed 3000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            // EstimatedBudget validation
            RuleFor(x => x.EstimatedBudget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Estimated budget must be greater than or equal to 0");

            // ActualBudget validation
            RuleFor(x => x.ActualBudget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Actual budget must be greater than or equal to 0");

            // WeatherSummary validation
            RuleFor(x => x.WeatherSummary)
                .MaximumLength(500)
                .WithMessage("Weather summary must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.WeatherSummary));
        }
    }
}
