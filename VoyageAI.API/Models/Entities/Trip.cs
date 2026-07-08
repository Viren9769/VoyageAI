namespace VoyageAI.API.Models.Entities
{
    public class Trip
    {
        public Guid TripId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string TripName { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationCity { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Currency { get; set; }
        public string TravelStyle { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }
        public ICollection<Traveler> Travelers { get; set; } = new List<Traveler>();
        public ICollection<ItineraryDay> ItineraryDays { get; set; } = new List<ItineraryDay>();
    }
}
