using FluentValidation;
using VoyageAI.API.DTOs.Auth;

namespace VoyageAI.API.Validators
{
    /// <summary>
    /// FluentValidation validator for refresh token requests.
    /// Ensures refresh token is provided and not empty.
    /// </summary>
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.")
                .WithErrorCode("REFRESH_TOKEN_REQUIRED");
        }
    }
}
