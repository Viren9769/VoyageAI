namespace VoyageAI.API.DTOs.Expenses
{
    public class ExpenseResponse
    {
        public Guid ExpenseId { get; set; }

        public Guid TripId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}