namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when user attempts to login with an unverified email address.
    /// HTTP Status: 403 Forbidden
    /// </summary>
    public class EmailNotVerifiedException : AppException
    {
        public EmailNotVerifiedException()
            : base(
                "Please verify your email address before logging in.",
                statusCode: 403,
                errorCode: "EMAIL_NOT_VERIFIED")
        {
        }
    }
}
