namespace VoyageAI.API.Common.Exceptions
{
    /// <summary>
    /// Thrown when a requested entity is not found.
    /// HTTP Status: 404 Not Found
    /// </summary>
    public class EntityNotFoundException : AppException
    {
        public EntityNotFoundException(string entityType, object id)
            : base(
                $"{entityType} with ID '{id}' not found.",
                statusCode: 404,
                errorCode: "ENTITY_NOT_FOUND")
        {
        }

        public EntityNotFoundException(string message)
            : base(
                message,
                statusCode: 404,
                errorCode: "ENTITY_NOT_FOUND")
        {
        }
    }
}
