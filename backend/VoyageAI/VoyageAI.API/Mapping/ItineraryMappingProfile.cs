using AutoMapper;
using VoyageAI.API.DTOs.Itinerary;
using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Mapping
{
    /// <summary>
    /// AutoMapper profile for itinerary day entity-to-DTO mappings.
    /// 
    /// This profile defines how ItineraryDay entities are mapped to/from DTOs.
    /// All mappings are bidirectional and include custom value transformations.
    /// 
    /// Mappings Defined:
    /// 1. ItineraryDay → ItineraryDayResponse (detailed response)
    /// 2. ItineraryDay → ItinerarySummaryResponse (lightweight response)
    /// 3. CreateItineraryDayRequest → ItineraryDay (on creation)
    /// 4. UpdateItineraryDayRequest → ItineraryDay (on update)
    /// 
    /// Design Pattern: Configuration-based mapping
    /// - Loose coupling: Service doesn't know about DTO structure details
    /// - Maintainability: Changes to DTOs don't require service code changes
    /// - Testability: Mappings can be tested independently
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddAutoMapper(typeof(Program).Assembly);
    /// This discovers all Profile implementations and registers them.
    /// 
    /// General Mapping Strategy:
    /// - Property names match exactly between entity and DTOs
    /// - AutoMapper handles the mapping automatically
    /// - Custom mappings are configured in ReverseMap() and ForMember() calls
    /// 
    /// Performance Notes:
    /// - Mappings are cached at startup for performance
    /// - No reflection overhead during request handling
    /// - Minimal memory overhead (one instance per assembly)
    /// </summary>
    public class ItineraryMappingProfile : Profile
    {
        public ItineraryMappingProfile()
        {
            // ============================================================
            // ItineraryDay → ItineraryDayResponse (Detailed)
            // ============================================================
            // Maps entity to detailed response DTO
            // Includes all fields including audit trail
            CreateMap<ItineraryDay, ItineraryDayResponse>()
                .ReverseMap();

            // ============================================================
            // ItineraryDay → ItinerarySummaryResponse (Lightweight)
            // ============================================================
            // Maps entity to summary response DTO
            // Omits audit trail details (CreatedAt, CreatedBy, LastModifiedBy, DeletedAt)
            // to reduce payload size for list views
            CreateMap<ItineraryDay, ItinerarySummaryResponse>()
                .ReverseMap();

            // ============================================================
            // CreateItineraryDayRequest → ItineraryDay
            // ============================================================
            // Maps create request to entity
            // Does NOT map:
            // - DayId: Generated in service
            // - CreatedAt: Set by service to UtcNow
            // - UpdatedAt: Set by service to UtcNow
            // - CreatedBy: Set by service from JWT
            // - LastModifiedBy: Set by service from JWT
            // - IsDeleted: Default to false
            // - DeletedAt: Default to null
            CreateMap<CreateItineraryDayRequest, ItineraryDay>()
                .ForMember(dest => dest.DayId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore())
                .ForMember(dest => dest.Activities, opt => opt.Ignore());

            // ============================================================
            // UpdateItineraryDayRequest → ItineraryDay
            // ============================================================
            // Maps update request to entity
            // Does NOT map:
            // - DayId: Immutable, identifies the resource
            // - TripId: Immutable, cannot move day to different trip
            // - CreatedAt: Immutable, set at creation
            // - CreatedBy: Immutable, set at creation
            // - UpdatedAt: Set by service to UtcNow
            // - LastModifiedBy: Set by service from JWT
            // - IsDeleted: Not modified by update (use DELETE endpoint instead)
            // - DeletedAt: Not modified by update (use DELETE endpoint instead)
            CreateMap<UpdateItineraryDayRequest, ItineraryDay>()
                .ForMember(dest => dest.DayId, opt => opt.Ignore())
                .ForMember(dest => dest.TripId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Trip, opt => opt.Ignore())
                .ForMember(dest => dest.Activities, opt => opt.Ignore());
        }
    }
}
