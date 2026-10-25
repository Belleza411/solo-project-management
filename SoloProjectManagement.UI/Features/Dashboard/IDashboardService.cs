using SoloProjectManagement.UI.Features.Dashboard.Dtos;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public interface IDashboardService
	{
		Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct);
		Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct);
		Task<List<RecentTaskDto>> GetRecentTasksAsync(CancellationToken ct);
	}
}
