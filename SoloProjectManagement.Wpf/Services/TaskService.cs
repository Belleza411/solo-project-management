using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Services
{
	public class TaskService(string connectionString) : ITaskService
	{
		public Task<ProjectTask> CreateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = await GetByIdAsync(id, ct);

			if (task is null) return false;

			db.ProjectTasks.Remove(task);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<ProjectTask>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			var tasks = await db.ProjectTasks
				.AsNoTracking()
				.ToListAsync(ct);

			var totalCount = tasks.Count;

			return new PagedResult<ProjectTask>(
				Items: tasks,
				PageNumber: pageNumber ?? 1,
				PageSize: pageSize ?? 3,
				TotalCount: totalCount);
		}

		public async Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
				.FirstOrDefaultAsync(t => t.Id == id, ct);
		}

		public Task<bool> UpdateAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
