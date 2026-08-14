using System.ComponentModel.DataAnnotations;

namespace VoyageAI.API.DTOs.Expenses
{
    public class CreateExpenseRequest
    {
        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Note { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}