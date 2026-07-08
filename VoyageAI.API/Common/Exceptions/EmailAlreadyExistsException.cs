namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when attempting to register with an email that already exists.
    /// HTTP Status: 409 Conflict
    /// </summary>
    public class EmailAlreadyExistsException : AppException
    {
        public EmailAlreadyExistsException(string email)
            : base(
                $"Email '{email}' is already registered.",
                statusCode: 409,
                errorCode: "EMAIL_ALREADY_EXISTS")
        {
        }
    }
}
