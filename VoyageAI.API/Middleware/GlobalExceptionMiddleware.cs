using System.Text.Json;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Common.Models;

namespace VoyageAI.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware.
    /// Catches all unhandled exceptions and returns standardized API responses.
    /// Never exposes internal exception details to clients (security best practice).
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions and returns appropriate API responses.
        /// </summary>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ApiResponse<object> response;
            int statusCode = 500;

            switch (exception)
            {
                // Custom application exceptions
                case AppException appEx:
                    {
                        statusCode = appEx.StatusCode;
                        var errors = appEx.Details?.Select(d => new ApiError(d, errorCode: appEx.ErrorCode)).ToList();
                        response = ApiResponse<object>.FailureResponse(appEx.Message, errors);
                        break;
                    }

                // Validation exceptions (from FluentValidation)
                case FluentValidation.ValidationException validationEx:
                    {
                        statusCode = 400;
                        var errors = validationEx.Errors
                            .Select(e => new ApiError(
                                e.ErrorMessage,
                                propertyName: e.PropertyName,
                                errorCode: e.ErrorCode))
                            .ToList();
                        response = ApiResponse<object>.FailureResponse("Validation failed", errors);
                        break;
                    }

                // Argument exceptions
                case ArgumentException argEx:
                    {
                        statusCode = 400;
                        response = ApiResponse<object>.FailureResponse(
                            "Invalid argument provided",
                            new List<ApiError> { new ApiError(argEx.Message, errorCode: "INVALID_ARGUMENT") });
                        break;
                    }

                // Unauthorized access
                case UnauthorizedAccessException:
                    {
                        statusCode = 401;
                        response = ApiResponse<object>.FailureResponse("Unauthorized access");
                        break;
                    }

                // Generic exceptions - never expose details
                default:
                    {
                        statusCode = 500;
                        response = ApiResponse<object>.FailureResponse(
                            "An unexpected error occurred. Please try again later.",
                            new List<ApiError> { new ApiError("Internal server error", errorCode: "INTERNAL_ERROR") });
                        break;
                    }
            }

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
