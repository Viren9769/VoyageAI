namespace VoyageAI.API.Common.Models
{
    /// <summary>
    /// Represents a single error in an API response.
    /// Used for validation errors, business rule violations, etc.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Property or field name where the error occurred.
        /// Null for general/non-field errors.
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// Human-readable error message.
        /// Never exposes internal exception details in production.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code for programmatic handling.
        /// Examples: "EMAIL_ALREADY_EXISTS", "INVALID_PASSWORD"
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Additional context information (optional).
        /// Can be used for UI hints or suggestions.
        /// </summary>
        public string? HelpText { get; set; }

        /// <summary>
        /// Creates an error with property name (for validation errors).
        /// </summary>
        public ApiError(string errorMessage, string? propertyName = null, string? errorCode = null, string? helpText = null)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
            ErrorCode = errorCode;
            HelpText = helpText;
        }
    }
}
