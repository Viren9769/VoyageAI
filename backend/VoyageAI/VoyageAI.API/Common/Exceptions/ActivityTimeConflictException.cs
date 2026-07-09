namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when an activity time conflicts with existing activities in the same day.
    /// HTTP Status: 409 Conflict
    /// </summary>
    public class ActivityTimeConflictException : AppException
    {
        public ActivityTimeConflictException(string message)
            : base(
                message,
                statusCode: 409,
                errorCode: "ACTIVITY_TIME_CONFLICT")
        {
        }
    }
}
