namespace VoyageAI.API.DTOs.Expenses
{
    public class BudgetComparisonResponse
    {
        public string Category { get; set; } = string.Empty;

        public decimal Budget { get; set; }

        public decimal Spent { get; set; }
    }
}