using VoyageAI.API.Models.Enums;

namespace VoyageAI.API.Models.Entities
{
    /// <summary>
    /// Represents a user in the VoyageAI application.
    /// Includes authentication, profile, and preference information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// User's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address (unique, used for login).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password using BCrypt.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// User's country code (ISO 3166-1 alpha-2, e.g., "US", "CA").
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// User's preferred currency code (e.g., "USD", "EUR").
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// User's preferred language code (e.g., "en", "fr").
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// User's preferred time zone (e.g., "UTC", "America/New_York").
        /// </summary>
        public string? TimeZone { get; set; } = "UTC";

        /// <summary>
        /// User's preferred UI theme ("light" or "dark").
        /// </summary>
        public string Theme { get; set; } = "light";

        /// <summary>
        /// URL to user's profile image.
        /// </summary>
        public string? ProfileImageUrl { get; set; }

        /// <summary>
        /// Account status (Active, Inactive, Suspended, Deleted).
        /// Replaces the old boolean IsActive field.
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Inactive;

        /// <summary>
        /// Whether the user's email address has been verified.
        /// Users cannot login until email is verified.
        /// </summary>
        public bool EmailVerified { get; set; } = false;

        /// <summary>
        /// Whether the user has completed their profile setup.
        /// Used to guide onboarding flow.
        /// </summary>
        public bool ProfileCompleted { get; set; } = false;

        /// <summary>
        /// When the user last successfully logged in (UTC).
        /// Null if user has never logged in.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// When this user account was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this user account was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Collection of trips created by this user.
        /// </summary>
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

        /// <summary>
        /// Collection of refresh tokens issued to this user.
        /// </summary>
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
