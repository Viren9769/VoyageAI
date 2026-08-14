namespace VoyageAI.API.Models.Entities
{
    public class Expense
    {
        public Guid ExpenseId { get; set; } = Guid.NewGuid();

        public Guid TripId { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public string Description { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid CreatedBy { get; set; }

        public Guid LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public Trip Trip { get; set; } = null!;
    }
}