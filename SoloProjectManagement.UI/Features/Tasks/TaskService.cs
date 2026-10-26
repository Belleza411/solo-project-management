using Microsoft.EntityFrameworkCore;
using SoloProjectManagement.UI.Common;
using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Models;
using SoloProjectManagement.UI.Persistence;

namespace SoloProjectManagement.UI.Features.Tasks
{
	public class TaskService(string connectionString) : ITaskService
	{
		public async Task<ProjectTask> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = new ProjectTask
			{
				ProjectId = request.ProjectId,
				Name = request.Name,
				Description = request.Description,
				Status = request.Status,
				Priority = request.Priority,
				DueDate = request.DueDate
			};

			db.ProjectTasks.Add(task);
			await db.SaveChangesAsync(ct);

			return task;
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = await GetByIdAsync(id, ct);

			if (task is null) return false;

			db.ProjectTasks.Remove(task);
			await db.SaveChangesAsync(ct);

			return true;
		}

		public async Task<PagedResult<ProjectTask>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var tasks = await db.ProjectTasks
				.AsNoTracking()
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(ct);

			var totalCount = tasks.Count;

			return new PagedResult<ProjectTask>(
				Items: tasks,
				PageNumber: pageNumber,
				PageSize: pageSize,
				TotalCount: totalCount);
		}

		public async Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			return await db.ProjectTasks
				.FirstOrDefaultAsync(t => t.Id == id, ct);
		}

		public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
		{
			using var db = new ProjectDbContext(connectionString);

			var task = await GetByIdAsync(id, ct);

			if (task is null) return false;

			task.ProjectId = request.ProjectId;
			task.Name = request.Name;
			task.Description = request.Description;
			task.Status = request.Status;
			task.Priority = request.Priority;
			task.DueDate = request.DueDate;
			task.UpdatedAt = DateTime.UtcNow;

			await db.SaveChangesAsync(ct);
			return true;
		}
	}
}
