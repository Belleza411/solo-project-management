using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Tasks
{
	public interface ITaskService
	{
		Task<PagedResult<ProjectTask>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken ct);
		Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<ProjectTask> CreateAsync(CreateTaskRequest request, CancellationToken ct);
		Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct);
		Task<bool> DeleteAsync(Guid id, CancellationToken ct);
	}
}
