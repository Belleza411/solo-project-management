using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Features.Dashboard.Dtos;
using SoloProjectManagement.UI.Persistence;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Dashboard
{
	public class DashboardService(string connectionString) : IDashboardService
	{
		public Task<List<MetricCardDto> > GetOverviewAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public async Task<List<ProjectCardDto>> GetRecentProjectsAsync(CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.Projects
				.AsNoTracking()
				.Select(p => new ProjectCardDto(
					p.Id,
					p.Title,
					p.Description ?? "",
					p.Status,
					p.DueDate,
					p.Tasks.Count(t => t.Status == TaskStatus.Completed),
					p.Tasks.Count()))
				.ToListAsync(ct);
		}
	}
}
