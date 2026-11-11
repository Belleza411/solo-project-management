using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Projects.Dtos;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Projects.Services
{
	public interface IProjectService
	{
		Task<PagedResult<ProjectDto>> GetAllAsync(
			ProjectsFilterRequest? request,
			int pageNumber = 1,
			int pageSize = 6,
			CancellationToken ct = default);
		Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<Project> CreateAsync(
			CreateProjectRequest request,
			CancellationToken ct = default);
		Task<bool> UpdateAsync(
			UpdateProjectRequest request,
			CancellationToken ct = default);
		Task<bool> DeleteAsync(Guid Id, CancellationToken ct = default);
	}
}