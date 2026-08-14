using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task CreateAsync(Expense expense, CancellationToken cancellationToken = default);

        Task<Expense?> GetByIdAsync(Guid expenseId, CancellationToken cancellationToken = default);

        Task<List<Expense>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

        Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}