using AutoMapper;
using VoyageAI.API.DTOs.Trip;
using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Mapping
{
    /// <summary>
    /// AutoMapper profile for Trip entity mappings.
    /// 
    /// This profile defines how to map between Trip entities and DTOs.
    /// AutoMapper automatically handles the conversion, reducing boilerplate code.
    /// 
    /// Mappings:
    /// 1. CreateTripRequest → Trip (for creating new trips)
    /// 2. UpdateTripRequest → Trip (for updating trips, via ForMember)
    /// 3. Trip → GetTripResponse (for returning trip data in responses)
    /// 
    /// Integration:
    /// Auto-registered in Program.cs via:
    /// services.AddAutoMapper(typeof(Program).Assembly);
    /// 
    /// Usage in Services:
    /// var trip = _mapper.Map{Trip}(createRequest);
    /// var response = _mapper.Map{GetTripResponse}(trip);
    /// </summary>
    public class TripMappingProfile : Profile
    {
        public TripMappingProfile()
        {
            // CreateTripRequest → Trip
            CreateMap<CreateTripRequest, Trip>()
                .ForMember(dest => dest.TripId, opt => opt.Ignore())  // ID is generated
                .ForMember(dest => dest.UserId, opt => opt.Ignore())  // Set by service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // Set by service
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())  // Set by service
                .ForMember(dest => dest.User, opt => opt.Ignore())  // Navigation property
                .ForMember(dest => dest.Travelers, opt => opt.Ignore())  // Navigation property
                .ForMember(dest => dest.ItineraryDays, opt => opt.Ignore());  // Navigation property

            // Trip → GetTripResponse
            CreateMap<Trip, GetTripResponse>();
        }
    }
}
