namespace VoyageAI.API.Common.Models
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// Provides consistent response structure across the application.
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// User-friendly message describing the result.
        /// Examples: "Login successful", "User created successfully"
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The actual data returned from the request.
        /// Can be null for operations that don't return data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Collection of errors if the request failed.
        /// Null if Success is true.
        /// </summary>
        public List<ApiError>? Errors { get; set; }

        /// <summary>
        /// Timestamp of when the response was generated (UTC).
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful API response.
        /// </summary>
        /// <param name="data">Data to return</param>
        /// <param name="message">Optional message</param>
        public static ApiResponse<T> SuccessResponse(T? data = default, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Operation successful",
                Data = data,
                Errors = null
            };
        }

        /// <summary>
        /// Creates a failed API response.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">List of detailed errors</param>
        public static ApiResponse<T> FailureResponse(string message, List<ApiError>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<ApiError>()
            };
        }
    }
}
