using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly VoyageDbContext _dbContext;

        public ExpenseRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task CreateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            await _dbContext.Expenses.AddAsync(expense, cancellationToken);
        }

        public async Task<Expense?> GetByIdAsync(Guid expenseId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .Include(e => e.Trip)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId, cancellationToken);
        }

        public async Task<List<Expense>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .Where(e => e.TripId == tripId && !e.IsDeleted)
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            _dbContext.Expenses.Update(expense);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}