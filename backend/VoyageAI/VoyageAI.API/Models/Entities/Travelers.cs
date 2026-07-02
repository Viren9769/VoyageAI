namespace VoyageAI.API.Models.Entities
{
    public class Traveler
    {
        public Guid TravelerId { get; set; } = Guid.NewGuid();
        public Guid TripId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public bool IsPrimaryTraveler { get; set; }

        // Navigation property
        public Trip Trip { get; set; }
    }
}
