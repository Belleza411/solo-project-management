using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Projects
{
	public interface IProjectService
	{
		Task<PagedResult<ProjectDto>> GetAllAsync(int pageNumber = 1, int pageSize = 6, CancellationToken ct = default);
		Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<Project> CreateAsync(
			string title,
			string? description,
			ProjectStatus status,
			DateTime dueDate,
			CancellationToken ct = default);
		Task<bool> UpdateAsync(
			Guid id, 
			string title,
			string? description,
			DateTime dueDate,
			ProjectStatus status,
			CancellationToken ct = default);
		Task<bool> DeleteAsync(Guid Id, CancellationToken ct = default);
	}
}