using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Services
{
	public interface IProjectService
	{
		Task<PagedResult<Project>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken ct);
		Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<Project> CreateAsync(
			string title,
			string? description,
			ProjectStatus status,
			CancellationToken ct);
		Task<bool> UpdateAsync(
			Guid id, 
			string title,
			string? description,
			ProjectStatus status,
			CancellationToken ct);
		Task<bool> DeleteAsync(Guid Id, CancellationToken ct);
	}
}