using AutoMapper;
using VoyageAI.API.DTOs.Auth;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Models.Enums;

namespace VoyageAI.API.Mapping
{
    /// <summary>
    /// AutoMapper profile for authentication-related mappings.
    /// 
    /// This profile defines all object-to-object mappings related to authentication.
    /// It eliminates boilerplate code and provides a single place to manage conversions
    /// between DTOs, entities, and other objects.
    /// 
    /// Mappings Defined:
    /// 1. RegisterRequest → User
    ///    Maps registration DTO to user entity
    ///    Handles auto-generated fields (UserId, timestamps, IsActive)
    ///    Ignores password field (hashed in service)
    /// 
    /// 2. User → UserDto
    ///    Maps full user entity to public DTO
    ///    Only exposes safe fields (excludes PasswordHash, sensitive fields)
    /// 
    /// Why AutoMapper?
    /// - Eliminates boilerplate mapping code
    /// - Centralized mapping configuration
    /// - Type-safe
    /// - Easy to test
    /// - Convention-based (matches properties by name)
    /// - Supports complex transformations
    /// - Single place to add new mappings
    /// 
    /// Dependency Injection:
    /// Registered in Program.cs as:
    /// services.AddAutoMapper(typeof(Program).Assembly);
    /// This automatically discovers and registers all Profile subclasses.
    /// 
    /// Usage in Services:
    /// var userDto = _mapper.Map{UserDto}(user);
    /// 
    /// Testing:
    /// Can be tested independently:
    /// var config = new MapperConfiguration(cfg => cfg.AddProfile{AuthMappingProfile}());
    /// var mapper = config.CreateMapper();
    /// var result = mapper.Map{UserDto}(user);
    /// </summary>
    public class AuthMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the AuthMappingProfile class.
        /// 
        /// Defines all AutoMapper mappings for authentication operations.
        /// </summary>
        public AuthMappingProfile()
        {
            // ============================================
            // RegisterRequest → User Mapping
            // ============================================
            CreateMap<RegisterRequest, User>()
                // UserId: Generate new GUID
                // The User entity initializes UserId to Guid.NewGuid() by default,
                // but we explicitly map to ensure it's a new ID
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))

                // FirstName: Map directly (same property name)
                // AutoMapper automatically handles this, but we can be explicit
                .ForMember(
                    dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.FirstName))

                // LastName: Map directly
                .ForMember(
                    dest => dest.LastName,
                    opt => opt.MapFrom(src => src.LastName))

                // Email: Map directly
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))

                // Phone: Map directly (handles null for optional field)
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Phone))

                // CountryCode: Map directly (handles null for optional field)
                .ForMember(
                    dest => dest.CountryCode,
                    opt => opt.MapFrom(src => src.Country))

                // PasswordHash: IGNORE (NOT mapped from DTO)
                // Password is hashed in the service layer using BCrypt
                // For security, we never accept a pre-hashed password from the client
                // The AuthService will:
                // 1. Receive plain password from RegisterRequest
                // 2. Hash it using BCrypt.HashPassword()
                // 3. Set user.PasswordHash to the hashed value
                // 4. Then map RegisterRequest → User for other fields
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore())

                // CreatedAt: Set to UTC now
                // Records when the user was created
                // Set automatically to DateTime.UtcNow at time of creation
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))

                // UpdatedAt: Set to UTC now
                // Records when the user was last updated
                // Initially same as CreatedAt
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))

                // Status: Set to Inactive
                // New users start as inactive until email is verified
                // Can be changed after email verification
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(_ => UserStatus.Inactive))

                // Currency: Map to default or ignore (not provided in registration)
                // Can be set later in profile settings
                .ForMember(
                    dest => dest.Currency,
                    opt => opt.Ignore())

                // Language: Ignore (not provided in registration)
                // Can be set later in profile settings
                .ForMember(
                    dest => dest.Language,
                    opt => opt.Ignore())

                // Theme: Ignore (not provided in registration)
                // Can be set later in profile settings
                .ForMember(
                    dest => dest.Theme,
                    opt => opt.Ignore())

                // ProfileImageUrl: Ignore (not provided in registration)
                // Users can upload profile image after registration
                .ForMember(
                    dest => dest.ProfileImageUrl,
                    opt => opt.Ignore())

                // Trips: Ignore (navigation property)
                // Will be populated via EF Core relationships
                .ForMember(
                    dest => dest.Trips,
                    opt => opt.Ignore());

            // ============================================
            // User → UserDto Mapping
            // ============================================
            CreateMap<User, UserDto>()
                // Properties are mapped automatically by name convention
                // No explicit configuration needed because:
                // - UserId → UserId ✓
                // - FirstName → FirstName ✓
                // - LastName → LastName ✓
                // - Email → Email ✓
                // - ProfileImageUrl → ProfileImageUrl ✓
                //
                // Properties NOT in UserDto are automatically ignored:
                // - PasswordHash (not exposed in API response)
                // - CreatedAt (not needed in login response)
                // - UpdatedAt (not needed in login response)
                // - IsActive (not exposed to client)
                // - Currency, Language, Theme (UI preferences, not in auth response)
                // - Trips (navigation property, not in auth response)
                //
                // This is why we don't need explicit configuration for this mapping;
                // AutoMapper's convention-based matching handles it perfectly.
                //
                // Example flow:
                // var user = await _userRepository.GetByEmailAsync(email, ct);
                // var userDto = _mapper.Map<UserDto>(user);
                // Result: Only UserId, FirstName, LastName, Email, ProfileImageUrl are copied
                ;
        }
    }
}
