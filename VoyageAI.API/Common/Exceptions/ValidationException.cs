namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when validation fails.
    /// HTTP Status: 400 Bad Request
    /// Can contain multiple validation errors.
    /// </summary>
    public class ValidationException : AppException
    {
        public ValidationException(string message, List<string>? details = null)
            : base(
                message,
                statusCode: 400,
                errorCode: "VALIDATION_ERROR",
                details: details)
        {
        }

        public ValidationException(Dictionary<string, string> errors)
            : base(
                "Validation failed.",
                statusCode: 400,
                errorCode: "VALIDATION_ERROR",
                details: errors.Values.ToList())
        {
        }
    }
}
