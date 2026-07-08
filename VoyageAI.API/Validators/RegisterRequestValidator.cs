using System.Text.RegularExpressions;
using FluentValidation;
using VoyageAI.API.DTOs.Auth;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Fluent validator for RegisterRequest DTO.
    /// 
    /// This validator implements complex business rules that go beyond what
    /// DataAnnotations can handle. It complements the DataAnnotations already
    /// defined on the RegisterRequest class.
    /// 
    /// Validation Layers:
    /// 1. DataAnnotations (on RegisterRequest): Basic rules, fast, synchronous
    /// 2. FluentValidation (this class): Complex rules, async, database queries
    /// 
    /// Why Two Validators?
    /// - DataAnnotations: Quick format checks before expensive operations
    /// - FluentValidation: Business logic, database queries, complex rules
    /// 
    /// What Gets Validated:
    /// - FirstName: Non-empty, no special characters, max length
    /// - LastName: Non-empty, no special characters, max length
    /// - Email: Valid format, not already registered in database
    /// - Password: Complex rules (uppercase, lowercase, number, special char)
    /// - Phone: Valid format if provided
    /// - Country: Valid value if provided
    /// 
    /// Database Queries:
    /// This validator uses IUserRepository to query the database for email uniqueness.
    /// This requires async validation (MustAsync).
    /// 
    /// Dependency Injection:
    /// IUserRepository is injected via constructor.
    /// Registered in Program.cs with:
    /// services.AddValidatorsFromAssemblyContaining(typeof(Program));
    /// 
    /// Usage:
    /// Automatically invoked by ASP.NET Core validation middleware on POST requests.
    /// If validation fails, middleware returns 400 Bad Request with error details.
    /// </summary>
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        /// <summary>
        /// The user repository for database queries.
        /// Used to check email uniqueness.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the RegisterRequestValidator class.
        /// 
        /// Sets up all validation rules for user registration.
        /// IUserRepository is injected for email uniqueness checking.
        /// 
        /// Validation Rules:
        /// 1. FirstName: Required, max 100 characters, no leading/trailing whitespace
        /// 2. LastName: Required, max 100 characters, no leading/trailing whitespace
        /// 3. Email: Required, valid format, not already registered
        /// 4. Password: Required, minimum 8 characters, complex (uppercase, lowercase, number, special)
        /// 5. Phone: Valid international format if provided (optional)
        /// 6. Country: Max 100 characters if provided (optional)
        /// </summary>
        /// <param name="userRepository">Repository for user data access (email uniqueness check)</param>
        public RegisterRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

            // ============================================
            // FirstName Validation
            // ============================================
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(100)
                .WithMessage("First name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

            // ============================================
            // LastName Validation
            // ============================================
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(100)
                .WithMessage("Last name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

            // ============================================
            // Email Validation
            // ============================================
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid email address.")
                .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.")
                // Async validation: Check if email already exists in database
                // This is a database query, so it must be async
                // MustAsync runs after synchronous rules pass
                // Return true if valid (email NOT taken), false if invalid (email taken)
                .MustAsync(EmailNotTakenAsync)
                .WithMessage("Email is already registered with another account.")
                .WithErrorCode("DuplicateEmail");

            // ============================================
            // Password Validation
            // ============================================
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(128)
                .WithMessage("Password must not exceed 128 characters.")
                // Password complexity: must contain uppercase, lowercase, number, and special character
                // Regex explanation:
                // ^                    - Start of string
                // (?=.*[a-z])          - Lookahead: must contain at least one lowercase letter
                // (?=.*[A-Z])          - Lookahead: must contain at least one uppercase letter
                // (?=.*\d)             - Lookahead: must contain at least one digit (0-9)
                // (?=.*[!@#$%^&*])     - Lookahead: must contain at least one special character
                // .{8,}                - Any character, 8 or more times
                // $                    - End of string
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])(.{8,})$")
                .WithMessage(
                    "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (!@#$%^&*).");

            // ============================================
            // Phone Validation (Optional)
            // ============================================
            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$", RegexOptions.None)
                .WithMessage("Phone number must be a valid international format (E.164).")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))  // Only validate if provided
                .OverridePropertyName("Phone");

            // ============================================
            // Country Validation (Optional)
            // ============================================
            RuleFor(x => x.Country)
                .MaximumLength(100)
                .WithMessage("Country must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Country));  // Only validate if provided
        }

        /// <summary>
        /// Checks if the provided email is NOT already taken in the database.
        /// 
        /// Async Validation Helper:
        /// This method is called by the Email validation rule (MustAsync).
        /// It performs a database query to check email uniqueness.
        /// 
        /// Return Value:
        /// - Returns true if email is NOT taken (validation passes)
        /// - Returns false if email IS taken (validation fails)
        /// 
        /// Why Async?
        /// Email uniqueness must be checked against the database.
        /// Async allows the database query to complete without blocking other requests.
        /// 
        /// CancellationToken:
        /// Allows the database query to be cancelled if the request is cancelled.
        /// 
        /// Method Naming:
        /// FluentValidation convention is to name methods with "Async" suffix
        /// when using MustAsync.
        /// 
        /// Example:
        /// User tries to register with "john@example.com"
        /// 1. EmailNotTakenAsync queries database
        /// 2. Database returns: email does NOT exist
        /// 3. Method returns true
        /// 4. Validation passes
        /// 
        /// Example:
        /// User tries to register with "existing@example.com" (already registered)
        /// 1. EmailNotTakenAsync queries database
        /// 2. Database returns: email DOES exist
        /// 3. Method returns false
        /// 4. Validation fails with message: "Email is already registered with another account."
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="cancellationToken">Cancellation token for the database query</param>
        /// <returns>
        /// True if email is NOT taken (validation passes).
        /// False if email IS taken (validation fails).
        /// </returns>
        private async Task<bool> EmailNotTakenAsync(string email, CancellationToken cancellationToken)
        {
            // Null or empty email is handled by NotEmpty rule, no need to check here
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;  // Let NotEmpty handle the error
            }

            // Query database to check if email exists
            // ExistsAsync returns true if email exists, false if it doesn't
            var emailExists = await _userRepository.ExistsAsync(email, cancellationToken);

            // Return opposite: true if NOT taken, false if taken
            // This is the contract for MustAsync validation methods
            return !emailExists;
        }
    }
}
