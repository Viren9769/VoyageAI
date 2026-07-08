namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when a user attempts to access or modify a resource they don't have permission for.
    /// HTTP Status: 403 Forbidden
    /// </summary>
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(
                message,
                statusCode: 403,
                errorCode: "FORBIDDEN")
        {
        }
    }
}
