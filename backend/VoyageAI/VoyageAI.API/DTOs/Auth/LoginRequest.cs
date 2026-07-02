using System.ComponentModel.DataAnnotations;

namespace VoyageAI.API.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for user login request.
    /// 
    /// This DTO defines the API contract for the POST /api/auth/login endpoint.
    /// It contains minimal credentials required to authenticate an existing user.
    /// 
    /// Validation:
    /// 1. Email must be provided and valid format
    /// 2. Password must be provided
    /// 
    /// Login Flow:
    /// 1. Client submits email and password
    /// 2. Service queries database for user by email
    /// 3. If user not found → 401 Unauthorized
    /// 4. If user found, verify password hash using BCrypt
    /// 5. If password incorrect → 401 Unauthorized
    /// 6. If password correct → Generate JWT token
    /// 7. Return LoginResponse with token and user info
    /// 
    /// Security Notes:
    /// - Password is provided in plain text by the client
    /// - Server-side password verification happens using BCrypt.Net
    /// - The provided password is never stored, only compared to the hash
    /// - No timing attacks via constant-time comparison (BCrypt handles this)
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// 
        /// Validation Rules:
        /// - Required (non-empty)
        /// - Must be a valid email format
        /// 
        /// This email is used to locate the user in the database.
        /// Comparison is case-insensitive to prevent duplicate accounts.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// 
        /// Validation Rules:
        /// - Required (non-empty)
        /// - Must match the password used at registration
        /// 
        /// Security Notes:
        /// - Password is provided in plain text
        /// - Server-side verification uses BCrypt.Net to compare against stored hash
        /// - BCrypt provides automatic defense against timing attacks
        /// - Original password from request body is never logged or stored
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(128, ErrorMessage = "Password must not exceed 128 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
