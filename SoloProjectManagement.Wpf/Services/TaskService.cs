using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Services
{
	public class TaskService(string connectionString) : ITaskService
	{
		public Task<ProjectTask> CreateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsyncGuid(Guid Id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<PagedResult<ProjectTask>> GetAllAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
