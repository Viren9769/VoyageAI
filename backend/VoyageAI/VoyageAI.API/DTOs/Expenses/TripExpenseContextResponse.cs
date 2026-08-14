namespace VoyageAI.API.DTOs.Expenses
{
    public class TripExpenseContextResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Days { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal Budget { get; set; }

        public string? CoverImage { get; set; }
    }
}