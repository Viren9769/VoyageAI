using FluentValidation;
using VoyageAI.API.DTOs.Auth;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// Fluent validator for LoginRequest DTO.
    /// 
    /// This validator implements validation rules for the login endpoint.
    /// It complements the DataAnnotations already defined on the LoginRequest class.
    /// 
    /// Validation Approach:
    /// - DataAnnotations (on LoginRequest): Basic format checks ([Required], [EmailAddress])
    /// - FluentValidation (this class): Additional business rules
    /// 
    /// What Gets Validated:
    /// - Email: Non-empty, valid format
    /// - Password: Non-empty, not excessively long
    /// 
    /// Note:
    /// Password complexity is NOT checked at login validation layer.
    /// Complexity rules apply at registration (RegisterRequestValidator).
    /// Login only needs:
    /// 1. Email exists and is valid format
    /// 2. Password is provided (actual verification happens in AuthService)
    /// 
    /// Dependency Injection:
    /// No dependencies required (unlike RegisterRequestValidator which needs IUserRepository).
    /// Registered automatically in Program.cs with:
    /// services.AddValidatorsFromAssemblyContaining(typeof(Program));
    /// 
    /// Usage:
    /// Automatically invoked by ASP.NET Core validation middleware on POST /api/auth/login.
    /// If validation fails, middleware returns 400 Bad Request with error details.
    /// </summary>
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        /// <summary>
        /// Initializes a new instance of the LoginRequestValidator class.
        /// 
        /// Sets up validation rules for user login.
        /// 
        /// Validation Rules:
        /// 1. Email: Required, valid email format
        /// 2. Password: Required, reasonably sized
        /// </summary>
        public LoginRequestValidator()
        {
            // ============================================
            // Email Validation
            // ============================================
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required for login.")
                .EmailAddress()
                .WithMessage("Email must be a valid email address.")
                .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.");

            // ============================================
            // Password Validation
            // ============================================
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required for login.")
                .MinimumLength(1)
                .WithMessage("Password must be provided.")
                .MaximumLength(128)
                .WithMessage("Password must not exceed 128 characters.");

            // Note: Password complexity is NOT validated here.
            // Complexity validation happens at registration (RegisterRequestValidator).
            // At login, we only verify that password matches the stored hash via BCrypt.
            // This prevents invalid login attempts due to strict complexity rules.
        }
    }
}
