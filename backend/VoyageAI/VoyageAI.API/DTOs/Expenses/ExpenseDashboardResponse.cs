namespace VoyageAI.API.DTOs.Expenses
{
    public class ExpenseDashboardResponse
    {
        public ExpenseSummaryResponse Summary { get; set; } = new();

        public List<ExpenseCategorySpendResponse> Categories { get; set; } = new();

        public List<BudgetComparisonResponse> BudgetComparison { get; set; } = new();

        public List<ExpenseResponse> Expenses { get; set; } = new();
    }
}