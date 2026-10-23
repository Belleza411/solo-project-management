using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Services
{
	public interface ITaskService
	{
		Task<PagedResult<ProjectTask>> GetAllAsync(CancellationToken ct);
		Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<ProjectTask> CreateAsync(CancellationToken ct);
		Task<bool> UpdateAsync(CancellationToken ct);
		Task<bool> DeleteAsyncGuid(Guid Id, CancellationToken ct);
	}
}
