using SoloProjectManagement.UI.Features.Dashboard.Dtos;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public interface IDashboardService
	{
		Task<List<MetricCardDto>> GetOverviewAsync(CancellationToken ct);
		Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct);
	}
}
