using FluentValidation;
using VoyageAI.API.DTOs.Travelers;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Validator for CreateTravelerRequest.
    /// 
    /// This validator ensures that when a user creates a new traveler on a trip, all required fields
    /// are provided and have valid values. FluentValidation provides a fluent, testable,
    /// and composable way to define validation rules.
    /// 
    /// Validation Rules:
    /// 1. FirstName - Required, 1-100 characters
    /// 2. LastName - Required, 1-100 characters
    /// 3. MiddleName - Optional, max 100 characters
    /// 4. DateOfBirth - Optional, must not be in the future, max age 120 years
    /// 5. Gender - Optional, max 50 characters
    /// 6. Email - Optional but must be valid if provided
    /// 7. Phone - Optional, max 20 characters
    /// 8. Nationality - Optional, max 100 characters
    /// 9. PassportNumber - Optional, max 50 characters
    /// 10. PassportCountry - Optional, max 100 characters
    /// 11. PassportExpiry - Optional, must not be in the past (for active trips)
    /// 12. EmergencyContactName - Optional, max 200 characters
    /// 13. EmergencyContactPhone - Optional, max 20 characters
    /// 14. Relationship - Optional, max 50 characters
    /// 15. DietaryPreference - Optional, max 500 characters
    /// 16. SpecialRequirements - Optional, max 1000 characters
    /// 17. FrequentFlyerNumber - Optional, max 100 characters
    /// 18. KnownTravelerNumber - Optional, max 100 characters
    /// 19. IsPrimaryTraveler - Optional, boolean
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    /// 
    /// Usage in Controller:
    /// The [ApiController] attribute automatically validates requests
    /// and returns 400 Bad Request if validation fails.
    /// </summary>
    public class CreateTravelerRequestValidator : AbstractValidator<CreateTravelerRequest>
    {
        /// <summary>
        /// Maximum age for a traveler (years).
        /// Used to validate DateOfBirth field.
        /// </summary>
        private const int MaxAge = 120;

        public CreateTravelerRequestValidator()
        {
            // FirstName validation - REQUIRED
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .Length(1, 100)
                .WithMessage("First name must be 1-100 characters");

            // LastName validation - REQUIRED
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(1, 100)
                .WithMessage("Last name must be 1-100 characters");

            // MiddleName validation - OPTIONAL
            RuleFor(x => x.MiddleName)
                .MaximumLength(100)
                .WithMessage("Middle name must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

            // DateOfBirth validation - OPTIONAL
            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date of birth cannot be in the future")
                .Must(BeValidAge)
                .WithMessage($"Traveler age cannot exceed {MaxAge} years")
                .When(x => x.DateOfBirth.HasValue);

            // Gender validation - OPTIONAL
            RuleFor(x => x.Gender)
                .MaximumLength(50)
                .WithMessage("Gender must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            // Email validation - OPTIONAL but must be valid if provided
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email address is not in a valid format")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // Phone validation - OPTIONAL
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            // Nationality validation - OPTIONAL
            RuleFor(x => x.Nationality)
                .MaximumLength(100)
                .WithMessage("Nationality must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Nationality));

            // PassportNumber validation - OPTIONAL
            RuleFor(x => x.PassportNumber)
                .MaximumLength(50)
                .WithMessage("Passport number must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PassportNumber));

            // PassportCountry validation - OPTIONAL
            RuleFor(x => x.PassportCountry)
                .MaximumLength(100)
                .WithMessage("Passport country must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PassportCountry));

            // PassportExpiry validation - OPTIONAL but must not be expired
            RuleFor(x => x.PassportExpiry)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Passport expiry date must not be in the past")
                .When(x => x.PassportExpiry.HasValue);

            // EmergencyContactName validation - OPTIONAL
            RuleFor(x => x.EmergencyContactName)
                .MaximumLength(200)
                .WithMessage("Emergency contact name must not exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactName));

            // EmergencyContactPhone validation - OPTIONAL
            RuleFor(x => x.EmergencyContactPhone)
                .MaximumLength(20)
                .WithMessage("Emergency contact phone must not exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone));

            // Relationship validation - OPTIONAL
            RuleFor(x => x.Relationship)
                .MaximumLength(50)
                .WithMessage("Relationship must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Relationship));

            // DietaryPreference validation - OPTIONAL
            RuleFor(x => x.DietaryPreference)
                .MaximumLength(500)
                .WithMessage("Dietary preference must not exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DietaryPreference));

            // SpecialRequirements validation - OPTIONAL
            RuleFor(x => x.SpecialRequirements)
                .MaximumLength(1000)
                .WithMessage("Special requirements must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequirements));

            // FrequentFlyerNumber validation - OPTIONAL
            RuleFor(x => x.FrequentFlyerNumber)
                .MaximumLength(100)
                .WithMessage("Frequent flyer number must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FrequentFlyerNumber));

            // KnownTravelerNumber validation - OPTIONAL
            RuleFor(x => x.KnownTravelerNumber)
                .MaximumLength(100)
                .WithMessage("Known traveler number must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.KnownTravelerNumber));
        }

        /// <summary>
        /// Validates that a traveler's age is within acceptable limits.
        /// A traveler cannot be older than the specified maximum age.
        /// </summary>
        /// <param name="dateOfBirth">The traveler's date of birth (nullable)</param>
        /// <returns>true if the age is valid, false otherwise</returns>
        private static bool BeValidAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                return true;

            var today = DateTime.UtcNow;
            var age = today.Year - dateOfBirth.Value.Year;

            // Account for whether birthday has occurred this year
            if (dateOfBirth.Value.Date > today.AddYears(-age))
                age--;

            return age <= MaxAge;
        }
    }
}
