using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Dashboard;

namespace VoyageAI.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardResponse>> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}