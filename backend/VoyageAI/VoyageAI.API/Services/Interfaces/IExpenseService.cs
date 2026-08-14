using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Expenses;

namespace VoyageAI.API.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<List<ExpenseResponse>>> GetExpensesAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseResponse>> GetExpenseByIdAsync(Guid userId, Guid tripId, Guid expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseResponse>> CreateExpenseAsync(Guid userId, Guid tripId, CreateExpenseRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseResponse>> UpdateExpenseAsync(Guid userId, Guid tripId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse<object>> DeleteExpenseAsync(Guid userId, Guid tripId, Guid expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseDashboardResponse>> GetDashboardAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default);
    }
}