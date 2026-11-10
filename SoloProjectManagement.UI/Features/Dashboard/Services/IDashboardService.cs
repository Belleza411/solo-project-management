using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;

namespace SoloProjectManagement.UI.Features.Dashboard.Services
{
	public interface IDashboardService
	{
		Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct = default);
		Task<List<ProjectDto>> GetRecentProjectsAsync(CancellationToken ct = default);
		Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct = default);
	}
}
