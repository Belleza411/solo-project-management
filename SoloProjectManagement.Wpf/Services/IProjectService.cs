using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Services
{
	public interface IProjectService
	{
		Task<PagedResult<Project>> GetAllAsync(CancellationToken ct);
		Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<Project> CreateAsync(CancellationToken ct);
		Task<bool> UpdateAsync(CancellationToken ct);
		Task<bool> DeleteAsyncGuid(Guid Id, CancellationToken ct);
	}
}