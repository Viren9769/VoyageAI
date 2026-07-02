namespace VoyageAI.API.Models.Entities
{
    public class ItineraryDay
    {
        public Guid DayId { get; set; } = Guid.NewGuid();
        public Guid TripId { get; set; } = Guid.NewGuid();
        public int DayNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Title { get; set; }
        public string Summary { get; set; }

        // Navigation properties
        public Trip Trip { get; set; }
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
