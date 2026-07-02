namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Base exception for all application-specific exceptions.
    /// Inherits from Exception and provides standard properties.
    /// </summary>
    public class AppException : Exception
    {
        /// <summary>
        /// HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Error code for programmatic handling.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// List of validation or related errors.
        /// </summary>
        public List<string> Details { get; set; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of AppException.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code (default: 500)</param>
        /// <param name="errorCode">Error code for identification</param>
        /// <param name="details">List of detailed error messages</param>
        public AppException(
            string message,
            int statusCode = 500,
            string? errorCode = null,
            List<string>? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details ?? new List<string>();
        }

        /// <summary>
        /// Initializes a new instance with inner exception.
        /// </summary>
        public AppException(
            string message,
            Exception innerException,
            int statusCode = 500,
            string? errorCode = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
