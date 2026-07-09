using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VoyageAI.API.Configuration;
using VoyageAI.API.Data;
using VoyageAI.API.Middleware;
using VoyageAI.API.Repositories;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services;
using VoyageAI.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURATION & OPTIONS
// ============================================================

// Get connection string from appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure JwtSettings from appsettings.json Jwt section
// This binds the configuration to the JwtSettings class
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ============================================================
// 2. DATABASE CONTEXT
// ============================================================

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<VoyageDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============================================================
// 3. REPOSITORY LAYER
// ============================================================

// Register repositories for dependency injection
// IUserRepository is the abstraction, UserRepository is the implementation
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register refresh token repository
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Register trip repository
builder.Services.AddScoped<ITripRepository, TripRepository>();

// Register traveler repository
builder.Services.AddScoped<ITravelerRepository, TravelerRepository>();

// Register itinerary repository
builder.Services.AddScoped<IItineraryRepository, ItineraryRepository>();

// Register activity repository
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();

// ============================================================
// 4. SERVICE LAYER
// ============================================================

// Register services for dependency injection
// IAuthService is the abstraction, AuthService is the implementation
builder.Services.AddScoped<IAuthService, AuthService>();

// Register trip service
builder.Services.AddScoped<ITripService, TripService>();

// Register traveler service
builder.Services.AddScoped<ITravelerService, TravelerService>();

// Register itinerary service
builder.Services.AddScoped<IItineraryService, ItineraryService>();

// Register activity service
builder.Services.AddScoped<IActivityService, ActivityService>();

// ============================================================
// 5. VALIDATION (FluentValidation)
// ============================================================

// Register all validators from the assembly
// This automatically discovers and registers all AbstractValidator<T> implementations
// Validators:
// - RegisterRequestValidator
// - LoginRequestValidator
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ============================================================
// 6. OBJECT MAPPING (AutoMapper)
// ============================================================

// Register AutoMapper and all mapping profiles from the assembly
// This automatically discovers and registers all Profile implementations
// Mapping Profile: AuthMappingProfile
//   - RegisterRequest → User
//   - User → UserDto
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ============================================================
// 7. CONTROLLERS & API
// ============================================================

builder.Services.AddControllers();

// ============================================================
// 8. AUTHENTICATION (JWT)
// ============================================================

// Get JWT settings from configuration
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

// Validate JWT settings before using them
if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
{
    throw new InvalidOperationException(
        "Invalid JWT configuration: Secret must be at least 32 characters long. " +
        "Check appsettings.json [Jwt:Secret]");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
{
    throw new InvalidOperationException(
        "Invalid JWT configuration: Issuer must be configured. " +
        "Check appsettings.json [Jwt:Issuer]");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException(
        "Invalid JWT configuration: Audience must be configured. " +
        "Check appsettings.json [Jwt:Audience]");
}

// Configure JWT Authentication
// This adds the JWT authentication scheme to the application
builder.Services.AddAuthentication(options =>
{
    // Set JWT Bearer as the default authentication scheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Create symmetric key from the secret
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

    // Configure JWT Bearer token validation
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the signing key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,

        // Validate the issuer (who created the token)
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        // Validate the audience (who the token is for)
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        // Validate the expiration time
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,  // No clock skew tolerance (strict time checking)

        // Validate name claim
        ValidateTokenReplay = false
    };

    // Configure JWT Bearer events for debugging and logging
    options.Events = new JwtBearerEvents
    {
        // Called when authentication fails
        OnAuthenticationFailed = context =>
        {
            // Log authentication failures for security monitoring
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },

        // Called when token is validated successfully
        OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        }
    };
});

// ============================================================
// 9. AUTHORIZATION
// ============================================================

// Add authorization services
// This enables [Authorize] attributes and policy-based authorization
builder.Services.AddAuthorizationBuilder()
    // Default policy: Require authenticated user
    .AddDefaultPolicy("default", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

// ============================================================
// 10. CORS (Cross-Origin Resource Sharing) - Optional
// ============================================================

// Uncomment if you need to allow requests from different origins
// This is important for frontend-backend separation
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
*/

// ============================================================
// 11. SWAGGER / OPENAPI (FOR API DOCUMENTATION)
// ============================================================

// Add Swagger/OpenAPI support for API documentation
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to use JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token in format: Bearer {token}"
    });

    // Require JWT token for authorized endpoints in Swagger UI
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add XML documentation for API
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();

// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development only
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VoyageAI API v1");
    });

    // In Development: Allow HTTP without forcing HTTPS redirect
    // This prevents certificate issues in local development
    // For Production: HTTPS redirection will be enforced
}
else
{
    // In Production: Enforce HTTPS redirection
    app.UseHttpsRedirection();
}

// Global exception handling middleware (must be early in the pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable CORS if configured (uncomment if CORS is added above)
// app.UseRouting();
// app.UseCors("AllowAll");

// Authentication middleware (extract JWT token from request)
// Must come before UseAuthorization
app.UseAuthentication();

// Authorization middleware (verify JWT token is valid)
// Must come after UseAuthentication
app.UseAuthorization();

// Map controllers (route requests to controllers)
app.MapControllers();

// ============================================================
// RUN APPLICATION
// ============================================================

app.Run();
