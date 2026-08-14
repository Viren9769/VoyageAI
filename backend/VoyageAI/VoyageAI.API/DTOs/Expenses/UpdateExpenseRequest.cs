using System.ComponentModel.DataAnnotations;

namespace VoyageAI.API.DTOs.Expenses
{
    public class UpdateExpenseRequest
    {
        public DateTime? ExpenseDate { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Amount { get; set; }
    }
}