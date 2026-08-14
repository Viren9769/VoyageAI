namespace VoyageAI.API.DTOs.Expenses
{
    public class ExpenseSummaryResponse
    {
        public decimal TotalBudget { get; set; }

        public decimal TotalSpent { get; set; }

        public decimal Remaining { get; set; }

        public decimal DailyAverage { get; set; }
    }
}