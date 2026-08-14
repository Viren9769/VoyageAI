namespace VoyageAI.API.DTOs.Expenses
{
    public class ExpenseCategorySpendResponse
    {
        public string Category { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Color { get; set; } = string.Empty;
    }
}