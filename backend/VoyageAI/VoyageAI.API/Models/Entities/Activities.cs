namespace VoyageAI.API.Models.Entities
{
    public class Activity
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public Guid DayId { get; set; } = Guid.NewGuid();
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string LocationName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal EstimatedCost { get; set; }
        public bool IsAiGenerated { get; set; }

        // Navigation property
        public ItineraryDay ItineraryDay { get; set; }
    }
}
