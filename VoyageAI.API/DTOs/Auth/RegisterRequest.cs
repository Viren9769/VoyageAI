using System.ComponentModel.DataAnnotations;

namespace VoyageAI.API.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for user registration request.
    /// 
    /// This DTO defines the API contract for the POST /api/auth/register endpoint.
    /// It is strictly separate from the User entity to provide security and flexibility.
    /// 
    /// Clients can only set safe fields. System-generated fields (UserId, CreatedAt, UpdatedAt, IsActive, etc.)
    /// are never accepted from the client and are handled server-side.
    /// 
    /// Validation occurs in two layers:
    /// 1. DataAnnotations (basic checks) - immediate feedback
    /// 2. FluentValidation via RegisterRequestValidator - complex rules including email uniqueness
    /// 
    /// After validation passes, the service layer maps this DTO to the User entity and hashes the password using BCrypt.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// 
        /// Validation Rules:
        /// - Required (non-empty)
        /// - Must be a valid string
        /// 
        /// This field is captured at registration and stored in the User entity.
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name.
        /// 
        /// Validation Rules:
        /// - Required (non-empty)
        /// - Must be a valid string
        /// 
        /// This field is captured at registration and stored in the User entity.
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// 
        /// Validation Rules:
        /// - Required (non-empty)
        /// - Must be a valid email format
        /// - Must be unique in the database (checked by FluentValidation)
        /// - If email exists, returns 409 Conflict response
        /// 
        /// Email serves as the unique identifier for user login and is case-insensitive.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// 
        /// Validation Rules (enforced by RegisterRequestValidator):
        /// - Required (non-empty)
        /// - Minimum 8 characters
        /// - At least one uppercase letter (A-Z)
        /// - At least one lowercase letter (a-z)
        /// - At least one numeric digit (0-9)
        /// - At least one special character (!@#$%^&*)
        /// 
        /// Security Notes:
        /// - Password is NEVER stored in plain text
        /// - Password is hashed using BCrypt algorithm before database persistence
        /// - Original password is discarded after hashing
        /// - Password cannot be retrieved; only verified during login
        /// - During login, the provided password is hashed and compared against the stored hash
        /// 
        /// Example valid passwords:
        /// - "MyPassword@123"
        /// - "SecurePass!999"
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number (optional).
        /// 
        /// Validation Rules:
        /// - Optional field (can be null or empty)
        /// - If provided, must be a valid phone format
        /// 
        /// This field can be updated later in the user profile settings.
        /// </summary>
        [Phone(ErrorMessage = "Phone must be a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters.")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the user's country (optional).
        /// 
        /// Validation Rules:
        /// - Optional field (can be null or empty)
        /// - If provided, must be a valid country value
        /// 
        /// This field can be updated later in the user profile settings and is used for
        /// localization, currency, and travel planning preferences.
        /// </summary>
        [StringLength(100, ErrorMessage = "Country must not exceed 100 characters.")]
        public string? Country { get; set; }
    }
}
