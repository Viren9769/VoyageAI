using AutoMapper;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Expenses;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    public class ExpenseService : IExpenseService
    {
        private static readonly string[] DefaultCategories =
        {
            "Accommodation",
            "Transportation",
            "Food & Dining",
            "Activities",
            "Shopping",
            "Others"
        };

        private static readonly string[] DefaultColors =
        {
            "#6d5efc",
            "#4ea3ff",
            "#58d17a",
            "#f8a333",
            "#f56585",
            "#b7c2d9"
        };

        private readonly IExpenseRepository _expenseRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(
            IExpenseRepository expenseRepository,
            ITripRepository tripRepository,
            IMapper mapper,
            ILogger<ExpenseService> logger)
        {
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<List<ExpenseResponse>>> GetExpensesAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);
                var expenses = await _expenseRepository.GetByTripIdAsync(tripId, cancellationToken);

                return ApiResponse<List<ExpenseResponse>>.SuccessResponse(
                    _mapper.Map<List<ExpenseResponse>>(expenses),
                    $"Retrieved {expenses.Count} expenses for trip {trip.TripId}");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<List<ExpenseResponse>>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<List<ExpenseResponse>>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses for trip {TripId}", tripId);
                return Failure<List<ExpenseResponse>>("An unexpected error occurred while retrieving expenses");
            }
        }

        public async Task<ApiResponse<ExpenseResponse>> GetExpenseByIdAsync(Guid userId, Guid tripId, Guid expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);
                var expense = await _expenseRepository.GetByIdAsync(expenseId, cancellationToken);

                if (expense == null || expense.TripId != tripId || expense.IsDeleted)
                {
                    throw new EntityNotFoundException($"Expense with ID {expenseId} not found");
                }

                return ApiResponse<ExpenseResponse>.SuccessResponse(_mapper.Map<ExpenseResponse>(expense), "Expense retrieved successfully");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense {ExpenseId} for trip {TripId}", expenseId, tripId);
                return Failure<ExpenseResponse>("An unexpected error occurred while retrieving the expense");
            }
        }

        public async Task<ApiResponse<ExpenseResponse>> CreateExpenseAsync(Guid userId, Guid tripId, CreateExpenseRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);

                var expense = _mapper.Map<Expense>(request);
                expense.ExpenseId = Guid.NewGuid();
                expense.TripId = tripId;
                expense.CreatedAt = DateTime.UtcNow;
                expense.UpdatedAt = DateTime.UtcNow;
                expense.CreatedBy = userId;
                expense.LastModifiedBy = userId;
                expense.IsDeleted = false;

                await _expenseRepository.CreateAsync(expense, cancellationToken);
                await _expenseRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Expense {ExpenseId} created for trip {TripId} by user {UserId}", expense.ExpenseId, trip.TripId, userId);

                return ApiResponse<ExpenseResponse>.SuccessResponse(_mapper.Map<ExpenseResponse>(expense), "Expense created successfully");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (ValidationException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense for trip {TripId}", tripId);
                return Failure<ExpenseResponse>("An unexpected error occurred while creating the expense");
            }
        }

        public async Task<ApiResponse<ExpenseResponse>> UpdateExpenseAsync(Guid userId, Guid tripId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);

                var expense = await _expenseRepository.GetByIdAsync(expenseId, cancellationToken);
                if (expense == null || expense.TripId != tripId || expense.IsDeleted)
                {
                    throw new EntityNotFoundException($"Expense with ID {expenseId} not found");
                }

                if (request.ExpenseDate.HasValue)
                    expense.ExpenseDate = request.ExpenseDate.Value;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    expense.Description = request.Description;

                if (request.Note != null)
                    expense.Note = request.Note;

                if (!string.IsNullOrWhiteSpace(request.Category))
                    expense.Category = request.Category;

                if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
                    expense.PaymentMethod = request.PaymentMethod;

                if (request.Amount.HasValue)
                    expense.Amount = request.Amount.Value;

                expense.UpdatedAt = DateTime.UtcNow;
                expense.LastModifiedBy = userId;

                await _expenseRepository.UpdateAsync(expense, cancellationToken);
                await _expenseRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<ExpenseResponse>.SuccessResponse(_mapper.Map<ExpenseResponse>(expense), "Expense updated successfully");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (ValidationException ex)
            {
                return Failure<ExpenseResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId} for trip {TripId}", expenseId, tripId);
                return Failure<ExpenseResponse>("An unexpected error occurred while updating the expense");
            }
        }

        public async Task<ApiResponse<object>> DeleteExpenseAsync(Guid userId, Guid tripId, Guid expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);

                var expense = await _expenseRepository.GetByIdAsync(expenseId, cancellationToken);
                if (expense == null || expense.TripId != tripId || expense.IsDeleted)
                {
                    throw new EntityNotFoundException($"Expense with ID {expenseId} not found");
                }

                expense.IsDeleted = true;
                expense.DeletedAt = DateTime.UtcNow;
                expense.UpdatedAt = DateTime.UtcNow;
                expense.LastModifiedBy = userId;

                await _expenseRepository.UpdateAsync(expense, cancellationToken);
                await _expenseRepository.SaveChangesAsync(cancellationToken);

                return ApiResponse<object>.SuccessResponse(null, "Expense deleted successfully");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<object>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<object>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId} for trip {TripId}", expenseId, tripId);
                return Failure<object>("An unexpected error occurred while deleting the expense");
            }
        }

        public async Task<ApiResponse<ExpenseDashboardResponse>> GetDashboardAsync(Guid userId, Guid tripId, CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await EnsureTripOwnershipAsync(userId, tripId, cancellationToken);
                var expenses = await _expenseRepository.GetByTripIdAsync(tripId, cancellationToken);

                var summary = BuildSummary(trip, expenses);
                var categories = BuildCategories(expenses);
                var budgetComparison = BuildBudgetComparison(trip, expenses);

                var dashboard = new ExpenseDashboardResponse
                {
                    Summary = summary,
                    Categories = categories,
                    BudgetComparison = budgetComparison,
                    Expenses = _mapper.Map<List<ExpenseResponse>>(expenses)
                };

                return ApiResponse<ExpenseDashboardResponse>.SuccessResponse(dashboard, "Expense dashboard retrieved successfully");
            }
            catch (EntityNotFoundException ex)
            {
                return Failure<ExpenseDashboardResponse>(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return Failure<ExpenseDashboardResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense dashboard for trip {TripId}", tripId);
                return Failure<ExpenseDashboardResponse>("An unexpected error occurred while retrieving the expense dashboard");
            }
        }

        private async Task<Trip> EnsureTripOwnershipAsync(Guid userId, Guid tripId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new EntityNotFoundException($"Trip with ID {tripId} not found");
            }

            if (trip.UserId != userId)
            {
                throw new ForbiddenException("You do not have permission to manage expenses on this trip.");
            }

            return trip;
        }

        private static ExpenseSummaryResponse BuildSummary(Trip trip, IReadOnlyCollection<Expense> expenses)
        {
            var totalSpent = expenses.Sum(expense => expense.Amount);
            var durationDays = Math.Max(1, (trip.EndDate.Date - trip.StartDate.Date).Days + 1);

            return new ExpenseSummaryResponse
            {
                TotalBudget = trip.Budget,
                TotalSpent = totalSpent,
                Remaining = Math.Max(trip.Budget - totalSpent, 0),
                DailyAverage = Math.Round(totalSpent / durationDays, 2)
            };
        }

        private static List<ExpenseCategorySpendResponse> BuildCategories(IEnumerable<Expense> expenses)
        {
            return DefaultCategories
                .Select((category, index) => new ExpenseCategorySpendResponse
                {
                    Category = category,
                    Amount = expenses.Where(expense => string.Equals(expense.Category, category, StringComparison.OrdinalIgnoreCase)).Sum(expense => expense.Amount),
                    Color = DefaultColors[index]
                })
                .ToList();
        }

        private static List<BudgetComparisonResponse> BuildBudgetComparison(Trip trip, IEnumerable<Expense> expenses)
        {
            var totalBudget = trip.Budget;
            var perCategoryBudget = Math.Round(totalBudget / DefaultCategories.Length, 2);

            return DefaultCategories
                .Select(category => new BudgetComparisonResponse
                {
                    Category = category,
                    Budget = perCategoryBudget,
                    Spent = expenses.Where(expense => string.Equals(expense.Category, category, StringComparison.OrdinalIgnoreCase)).Sum(expense => expense.Amount)
                })
                .ToList();
        }

        private static ApiResponse<T> Failure<T>(string message)
        {
            return ApiResponse<T>.FailureResponse(message, new List<ApiError> { new ApiError(message) });
        }
    }
}