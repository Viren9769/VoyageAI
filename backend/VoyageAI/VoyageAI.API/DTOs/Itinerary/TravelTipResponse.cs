namespace VoyageAI.API.DTOs.Itinerary
{
    public class TravelTipResponse
    {
        public Guid Id { get; set; }

        public Guid TripId { get; set; }

        public int DayNumber { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public decimal TemperatureCelsius { get; set; }

        public string Icon { get; set; } = string.Empty;

        public string RecommendedWindow { get; set; } = string.Empty;

        public string Source { get; set; } = "Voyage AI Assistant";

        public decimal AiConfidence { get; set; }
    }
}