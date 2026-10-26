using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.ActivityLogs
{
	public interface IActivityLogService
	{
		Task<PagedResult<ActivityLog>> GetRecentAsync(int pageNumber = 1, int pageSize = 10, Guid? projectId = null, CancellationToken ct = default);
	}
}
