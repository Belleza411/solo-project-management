using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Features.Tasks
{
	public interface ITaskService
	{
		Task<PagedResult<ProjectTask>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
		Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<ProjectTask> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
		Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default);
		Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
	}
}
