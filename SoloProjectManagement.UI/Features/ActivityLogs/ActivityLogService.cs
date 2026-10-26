using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Features.ActivityLogs
{
	public class ActivityLogService(string connectionString) : IActivityLogService
	{
		public async Task<PagedResult<ActivityLog>> GetRecentAsync(int pageNumber = 1, int pageSize = 10, Guid? projectId = null, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var query = db.ActivityLogs.AsNoTracking();

			if (projectId.HasValue)
				query = query.Where(a => a.ProjectId == projectId);

			var activityLogs = await query
				.OrderByDescending(a => a.CreatedAt)
				.ThenByDescending(a => a.Id)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(ct);

			var totalCount = query.Count();

			return new PagedResult<ActivityLog>(
				Items: activityLogs,
				PageNumber: pageNumber,
				PageSize: pageSize,
				TotalCount: totalCount);
		}
	}
}
