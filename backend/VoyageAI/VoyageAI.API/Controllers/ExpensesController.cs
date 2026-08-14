using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Expenses;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/expenses")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
        {
            _expenseService = expenseService ?? throw new ArgumentNullException(nameof(expenseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ExpenseResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<ExpenseResponse>>>> GetExpenses(Guid tripId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<List<ExpenseResponse>>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.GetExpensesAsync(userId.Value, tripId, cancellationToken);
            return ToActionResult(response);
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<ExpenseDashboardResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ExpenseDashboardResponse>>> GetDashboard(Guid tripId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<ExpenseDashboardResponse>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.GetDashboardAsync(userId.Value, tripId, cancellationToken);
            return ToActionResult(response);
        }

        [HttpGet("{expenseId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ExpenseResponse>>> GetExpense(Guid tripId, Guid expenseId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<ExpenseResponse>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.GetExpenseByIdAsync(userId.Value, tripId, expenseId, cancellationToken);
            return ToActionResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<ExpenseResponse>>> CreateExpense(Guid tripId, [FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<ExpenseResponse>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.CreateExpenseAsync(userId.Value, tripId, request, cancellationToken);
            if (response.Success && response.Data != null)
            {
                return Created($"/api/trips/{tripId}/expenses/{response.Data.ExpenseId}", response);
            }

            return ToActionResult(response);
        }

        [HttpPut("{expenseId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ExpenseResponse>>> UpdateExpense(Guid tripId, Guid expenseId, [FromBody] UpdateExpenseRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<ExpenseResponse>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.UpdateExpenseAsync(userId.Value, tripId, expenseId, request, cancellationToken);
            return ToActionResult(response);
        }

        [HttpDelete("{expenseId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteExpense(Guid tripId, Guid expenseId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<object>.FailureResponse("User ID not found in token"));

            var response = await _expenseService.DeleteExpenseAsync(userId.Value, tripId, expenseId, cancellationToken);
            if (response.Success)
            {
                return NoContent();
            }

            return ToActionResult(response);
        }

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return null;
            }

            return userId;
        }

        private ActionResult ToActionResult<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            return BadRequest(response);
        }
    }
}