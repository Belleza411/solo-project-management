using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;

namespace SoloProjectManagement.UI.Services
{
	public class ProjectService(string connectionString) : IProjectService
	{
		public Task<Project> CreateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsyncGuid(Guid Id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<PagedResult<Project>> GetAllAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
