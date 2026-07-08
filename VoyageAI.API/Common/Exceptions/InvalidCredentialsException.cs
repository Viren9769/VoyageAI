namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when login credentials are invalid (email not found or password incorrect).
    /// HTTP Status: 401 Unauthorized
    /// Generic message for security (no user enumeration).
    /// </summary>
    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException()
            : base(
                "Invalid email or password.",
                statusCode: 401,
                errorCode: "INVALID_CREDENTIALS")
        {
        }
    }
}
