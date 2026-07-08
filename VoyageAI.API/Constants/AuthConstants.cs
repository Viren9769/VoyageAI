namespace VoyageAI.API.Constants
{
    /// <summary>
    /// Authentication and authorization related constants.
    /// Eliminates magic strings from the codebase.
    /// </summary>
    public static class AuthConstants
    {
        /// <summary>
        /// JWT Claims
        /// </summary>
        public static class JwtClaims
        {
            public const string UserId = "sub";
            public const string Email = "email";
            public const string FirstName = "firstName";
            public const string LastName = "lastName";
            public const string Role = "role";
        }

        /// <summary>
        /// JWT Configuration keys
        /// </summary>
        public static class JwtConfig
        {
            public const string ConfigKey = "Jwt";
            public const string Secret = "Secret";
            public const string Issuer = "Issuer";
            public const string Audience = "Audience";
            public const string ExpiryMinutes = "ExpiryMinutes";
            public const int DefaultExpiryMinutes = 60;
            public const int DefaultRefreshTokenExpiryDays = 7;
        }

        /// <summary>
        /// Password requirements
        /// </summary>
        public static class PasswordRequirements
        {
            public const int MinimumLength = 8;
            public const int MaximumLength = 128;
            public const string Regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])(.{8,})$";
            public const string RegexMessage = "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character.";
        }

        /// <summary>
        /// Email requirements
        /// </summary>
        public static class EmailRequirements
        {
            public const int MaximumLength = 256;
        }

        /// <summary>
        /// Name requirements
        /// </summary>
        public static class NameRequirements
        {
            public const int MaximumLength = 100;
        }

        /// <summary>
        /// Phone requirements
        /// </summary>
        public static class PhoneRequirements
        {
            public const int MaximumLength = 20;
            public const string Regex = @"^\+?[1-9]\d{1,14}$";
            public const string RegexMessage = "Phone number must be in valid international format (E.164).";
        }

        /// <summary>
        /// Country code requirements
        /// </summary>
        public static class CountryCodeRequirements
        {
            public const int MaximumLength = 2;
        }

        /// <summary>
        /// Token expiry times
        /// </summary>
        public static class TokenExpiry
        {
            public const int AccessTokenMinutes = 60;
            public const int RefreshTokenDays = 7;
            public const int EmailVerificationTokenHours = 24;
            public const int PasswordResetTokenHours = 1;
        }

        /// <summary>
        /// Error messages
        /// </summary>
        public static class ErrorMessages
        {
            public const string EmailAlreadyExists = "Email address is already registered.";
            public const string InvalidCredentials = "Invalid email or password.";
            public const string EmailNotVerified = "Please verify your email address before logging in.";
            public const string AccountSuspended = "Your account has been suspended.";
            public const string AccountNotFound = "User account not found.";
            public const string InvalidToken = "Invalid or expired token.";
            public const string TokenNotFound = "Token not found.";
            public const string OperationFailed = "Operation failed. Please try again.";
            public const string UnauthorizedAccess = "Unauthorized access.";
        }

        /// <summary>
        /// Success messages
        /// </summary>
        public static class SuccessMessages
        {
            public const string RegistrationSuccessful = "Registration successful. Please verify your email to activate your account.";
            public const string LoginSuccessful = "Login successful.";
            public const string TokenRefreshed = "Token refreshed successfully.";
            public const string PasswordResetEmailSent = "Password reset link sent to your email.";
            public const string PasswordResetSuccessful = "Password reset successful. Please login with your new password.";
            public const string EmailVerificationEmailSent = "Verification email sent. Please check your inbox.";
            public const string EmailVerifiedSuccessful = "Email verified successfully. Your account is now active.";
        }

        /// <summary>
        /// Default values
        /// </summary>
        public static class Defaults
        {
            public const string DefaultLanguage = "en";
            public const string DefaultCurrency = "USD";
            public const string DefaultTimeZone = "UTC";
        }

        /// <summary>
        /// Policy names for authorization
        /// </summary>
        public static class Policies
        {
            public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
        }
    }
}
