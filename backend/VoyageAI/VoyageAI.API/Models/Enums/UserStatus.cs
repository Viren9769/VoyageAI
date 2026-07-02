namespace VoyageAI.API.Models.Enums
{
    /// <summary>
    /// Represents the status of a user account.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User account is active and can access the application.
        /// </summary>
        Active = 1,

        /// <summary>
        /// User account is inactive. User created account but hasn't verified email.
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// User account is suspended due to policy violation or suspicious activity.
        /// </summary>
        Suspended = 3,

        /// <summary>
        /// User account is deleted (soft delete). Data retained for compliance.
        /// </summary>
        Deleted = 4
    }
}
