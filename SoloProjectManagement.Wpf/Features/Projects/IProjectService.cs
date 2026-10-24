using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Projects
{
	public interface IProjectService
	{
		Task<PagedResult<ProjectDto> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken ct);
		Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<Project> CreateAsync(
			string title,
			string? description,
			ProjectStatus status,
			DateTime dueDate,
			CancellationToken ct);
		Task<bool> UpdateAsync(
			Guid id, 
			string title,
			string? description,
			DateTime dueDate,
			ProjectStatus status,
			CancellationToken ct);
		Task<bool> DeleteAsync(Guid Id, CancellationToken ct);
	}
}