namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when user account has been suspended.
    /// HTTP Status: 403 Forbidden
    /// </summary>
    public class AccountSuspendedException : AppException
    {
        public AccountSuspendedException()
            : base(
                "Your account has been suspended. Please contact support.",
                statusCode: 403,
                errorCode: "ACCOUNT_SUSPENDED")
        {
        }
    }
}
