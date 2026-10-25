using SoloProjectManagement.UI.Features.Dashboard.Dtos;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public interface IDashboardService
	{
		Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct = default);
		Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct = default);
		Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct = default);
	}
}
